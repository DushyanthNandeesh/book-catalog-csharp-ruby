#!/usr/bin/env bash
# Runs the same scripted input through the C# and Ruby programs and checks that
#   1. the text printed on screen is identical, and
#   2. the JSON file each program saves is identical.
# Usage (from the repository root):  bash tests/compare.sh
# Needs: ruby and the .NET 8 SDK on your PATH.

set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
TESTS="$ROOT/tests"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

echo "Building C# program..."
# DOTNET_BUILD_ARGS is optional extra build arguments (normally leave it empty).
dotnet build "$ROOT/csharp/BookCatalog" -v q --nologo ${DOTNET_BUILD_ARGS:-} > "$TMP/build.log" 2>&1 \
  || { cat "$TMP/build.log"; echo "C# build failed."; exit 1; }
CS_DLL="$ROOT/csharp/BookCatalog/bin/Debug/net8.0/BookCatalog.dll"

pass=0; fail=0
while read -r scenario fixture; do
  case "$scenario" in ""|\#*) continue ;; esac
  input="$TESTS/inputs/$scenario.txt"

  for lang in csharp ruby; do
    if [ "$fixture" = "NONE" ]; then
      data="$TMP/$lang-$scenario-none/books.json"      # folder and file do not exist yet
    else
      data="$TMP/$lang-$scenario-$fixture"
      cp "$TESTS/fixtures/$fixture" "$data"
    fi
    if [ "$lang" = "csharp" ]; then
      dotnet "$CS_DLL" "$data" < "$input" > "$TMP/$lang.out"
    else
      ruby "$ROOT/ruby/main.rb" "$data" < "$input" > "$TMP/$lang.out"
    fi
    echo "$data" > "$TMP/$lang.path"
  done

  ok=1
  diff -q "$TMP/csharp.out" "$TMP/ruby.out" > /dev/null || { ok=0; echo "  screen output differs"; }
  cs_json="$(cat "$(cat "$TMP/csharp.path")" 2>/dev/null)"
  rb_json="$(cat "$(cat "$TMP/ruby.path")" 2>/dev/null)"
  [ "$cs_json" = "$rb_json" ] || { ok=0; echo "  saved JSON differs"; }

  if [ $ok -eq 1 ]; then pass=$((pass+1)); echo "PASS  $scenario  ($fixture)"
  else fail=$((fail+1)); echo "FAIL  $scenario  ($fixture)"; diff "$TMP/csharp.out" "$TMP/ruby.out" | head -10; fi
done < "$TESTS/scenarios.txt"

echo "----"
echo "Passed: $pass   Failed: $fail"
[ $fail -eq 0 ]
