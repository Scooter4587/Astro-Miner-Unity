import { execFileSync } from "node:child_process";
import { readFileSync } from "node:fs";
import { pathToFileURL } from "node:url";

const VERSION_HEADING = /^## \[([^\]]+)\](?: - (\d{4}-\d{2}-\d{2}))?\s*$/;
const SECTION_HEADING = /^### (.+?)\s*$/;
const DISCORD_FIELD_LIMIT = 1024;
const MAX_FIELDS = 8;

export function parseChangelog(markdown) {
  const releases = [];
  let release = null;
  let section = null;

  for (const rawLine of markdown.split(/\r?\n/)) {
    const versionMatch = rawLine.match(VERSION_HEADING);

    if (versionMatch) {
      if (release) releases.push(release);

      release = {
        version: versionMatch[1].trim(),
        date: versionMatch[2] ?? null,
        sections: [],
      };
      section = null;
      continue;
    }

    if (!release) continue;

    const sectionMatch = rawLine.match(SECTION_HEADING);

    if (sectionMatch) {
      section = { name: sectionMatch[1].trim(), entries: [] };
      release.sections.push(section);
      continue;
    }

    if (!section) continue;

    const trimmed = rawLine.trim();

    if (trimmed.startsWith("- ")) {
      section.entries.push(trimmed.slice(2).trim());
      continue;
    }

    if (/^\[[^\]]+\]:\s/.test(trimmed)) continue;

    if (trimmed && section.entries.length > 0 && !trimmed.startsWith(">")) {
      const lastIndex = section.entries.length - 1;
      section.entries[lastIndex] += ` ${trimmed}`;
    }
  }

  if (release) releases.push(release);

  return releases.filter(
    (candidate) => candidate.version.toLowerCase() !== "unreleased",
  );
}

export function selectRelease(currentMarkdown, previousMarkdown, explicitVersion) {
  const releases = parseChangelog(currentMarkdown);

  if (explicitVersion) {
    const normalizedVersion = explicitVersion.trim().replace(/^v/i, "");
    return releases.find((release) => release.version === normalizedVersion) ?? null;
  }

  const previousVersions = new Set(
    parseChangelog(previousMarkdown).map((release) => release.version),
  );

  return releases.find((release) => !previousVersions.has(release.version)) ?? null;
}

function truncate(value, limit) {
  if (value.length <= limit) return value;
  return `${value.slice(0, Math.max(0, limit - 1)).trimEnd()}…`;
}

export function buildDiscordPayload(release, repositoryUrl) {
  const fields = release.sections
    .filter((section) => section.entries.length > 0)
    .slice(0, MAX_FIELDS)
    .map((section) => ({
      name: section.name,
      value: truncate(
        section.entries.map((entry) => `• ${entry}`).join("\n"),
        DISCORD_FIELD_LIMIT,
      ),
      inline: false,
    }));

  return {
    username: "Astro Miner Changelog",
    allowed_mentions: { parse: [] },
    embeds: [
      {
        title: "🚀 Astro Miner",
        url: `${repositoryUrl}/blob/main/CHANGELOG.md`,
        description: `**${release.version}**${release.date ? ` • ${release.date}` : ""}`,
        color: 0xf59e0b,
        fields,
        footer: { text: "Astro Miner • main" },
      },
    ],
  };
}

function readPreviousChangelog(beforeSha) {
  if (!beforeSha || /^0+$/.test(beforeSha)) return "";

  try {
    return execFileSync("git", ["show", `${beforeSha}:CHANGELOG.md`], {
      encoding: "utf8",
      stdio: ["ignore", "pipe", "ignore"],
    });
  } catch {
    return "";
  }
}

function isDiscordWebhookUrl(value) {
  return /^https:\/\/(?:discord\.com|discordapp\.com)\/api\/webhooks\//.test(value);
}

async function main() {
  const currentMarkdown = readFileSync("CHANGELOG.md", "utf8");
  const explicitVersion = process.env.CHANGELOG_VERSION?.trim() ?? "";
  const previousMarkdown = explicitVersion
    ? ""
    : readPreviousChangelog(process.env.BEFORE_SHA ?? "");
  const release = selectRelease(currentMarkdown, previousMarkdown, explicitVersion);

  if (!release) {
    console.log("No new changelog version was found; Discord notification skipped.");
    return;
  }

  const repositoryUrl = `${process.env.GITHUB_SERVER_URL ?? "https://github.com"}/${process.env.GITHUB_REPOSITORY ?? "Scooter4587/Astro-Miner-Unity"}`;
  const payload = buildDiscordPayload(release, repositoryUrl);

  if (process.argv.includes("--dry-run")) {
    console.log(JSON.stringify(payload, null, 2));
    return;
  }

  const webhookUrl = process.env.DISCORD_WEBHOOK_URL?.trim() ?? "";

  if (!webhookUrl) {
    throw new Error("DISCORD_CHANGELOG_WEBHOOK_URL is not configured.");
  }

  if (!isDiscordWebhookUrl(webhookUrl)) {
    throw new Error("The configured Discord webhook URL is not an approved Discord webhook endpoint.");
  }

  const deliveryUrl = new URL(webhookUrl);
  deliveryUrl.searchParams.set("wait", "true");

  let response;
  try {
    response = await fetch(deliveryUrl, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(payload),
      signal: AbortSignal.timeout(15000),
    });
  } catch {
    throw new Error("Discord webhook request failed or timed out; webhook details are not logged.");
  }

  if (!response.ok) {
    throw new Error(`Discord webhook delivery failed with HTTP ${response.status}.`);
  }

  const message = await response.json();
  if (!message.id) {
    throw new Error("Discord did not confirm creation of the changelog message.");
  }

  console.log(`Discord changelog notification sent for ${release.version}.`);
}

const entryPoint = process.argv[1]
  ? pathToFileURL(process.argv[1]).href
  : "";

if (import.meta.url === entryPoint) {
  await main();
}
