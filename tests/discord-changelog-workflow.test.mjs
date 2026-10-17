import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import test from "node:test";
import { buildDiscordPayload, parseChangelog, selectRelease } from "../scripts/post-discord-changelog.mjs";

const previous = "## [0.0.1] - 2026-10-17\n### Added\n- Development environment.\n";
const current = "## [Unreleased]\n### Added\n- Work in progress.\n\n## [0.0.2] - 2026-10-18\n### Added\n- Flight test.\n\n" + previous;

test("parses releases and ignores Unreleased", () => {
  assert.deepEqual(parseChangelog(current).map(release => release.version), ["0.0.2", "0.0.1"]);
  const releases = parseChangelog(readFileSync("CHANGELOG.md", "utf8"));
  assert.ok(releases.length > 0);
  assert.ok(releases[0].sections.some(section => section.entries.length > 0));
  assert.equal(new Set(releases.map(release => release.version)).size, releases.length);
});

test("selects the new release and skips edits to an existing version", () => {
  assert.equal(selectRelease(current, previous, "")?.version, "0.0.2");
  assert.equal(selectRelease(current, current, ""), null);
  assert.equal(selectRelease(previous.replace("Development environment", "Corrected environment wording"), previous, ""), null);
});

test("Unreleased changes do not create a notification", () => {
  const workInProgress = "## [Unreleased]\n### Changed\n- More work.\n\n" + previous;
  assert.equal(selectRelease(workInProgress, previous, ""), null);
});

test("manual selection accepts a leading v and only existing versions", () => {
  assert.equal(selectRelease(current, "", "v0.0.1")?.version, "0.0.1");
  assert.equal(selectRelease(current, "", "9.9.9"), null);
});

test("payload uses Astro Miner branding, the main changelog link, and no mentions", () => {
  const release = selectRelease(current, previous, "");
  const payload = buildDiscordPayload(release, "https://github.com/example/astro-miner");
  assert.equal(payload.username, "Astro Miner Changelog");
  assert.equal(payload.embeds[0].title, "🚀 Astro Miner");
  assert.equal(payload.embeds[0].url, "https://github.com/example/astro-miner/blob/main/CHANGELOG.md");
  assert.equal(payload.embeds[0].footer.text, "Astro Miner • main");
  assert.deepEqual(payload.allowed_mentions, { parse: [] });
});

test("long sections fit Discord field limits and empty sections are omitted", () => {
  const release = { version: "0.0.1", sections: [{ name: "Added", entries: ["x".repeat(2000)] }, { name: "Fixed", entries: [] }] };
  const payload = buildDiscordPayload(release, "https://github.com/example/astro-miner");
  assert.equal(payload.embeds[0].fields.length, 1);
  assert.equal(payload.embeds[0].fields[0].value.length, 1024);
});
