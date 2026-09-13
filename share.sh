#!/usr/bin/env bash

set -euo pipefail

if ! curl --fail --silent http://localhost:5173 >/dev/null; then
  echo "The application is not running. Start ./run.sh in another Terminal window first."
  exit 1
fi

if ! command -v cloudflared >/dev/null 2>&1; then
  echo "cloudflared is not installed."
  exit 1
fi

echo "Creating a temporary public URL…"
echo "Keep this Terminal window open. Press Control-C to stop sharing."
cloudflared tunnel --url http://localhost:5173
