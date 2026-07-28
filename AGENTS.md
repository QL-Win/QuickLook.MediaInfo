# AGENTS.md

Instructions for AI coding agents working in this repository.

## Skills

Project agent skills live in **`.github/skills/`**. Treat this directory as the skills root for this repo.

- Each skill is a subdirectory containing a `SKILL.md` file.
- Before starting relevant work, discover available skills under `.github/skills/`, read matching `SKILL.md` files, and follow their instructions.
- Prefer project skills over ad-hoc approaches when a skill applies.
- When creating or updating skills, place them under `.github/skills/<skill-name>/SKILL.md` (do not use `.cursor/skills/` or `.agents/skills/` for this repo).

### Available skills

| Skill | Path | Use when |
| ----- | ---- | -------- |
| `update-mediainfo-languages` | `.github/skills/update-mediainfo-languages/` | Sync language enum/resources against a MediaInfo release (latest or user-specified) |
