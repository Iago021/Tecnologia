#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

python3 - <<'PY'
from pathlib import Path

# BlockAndTintGetter is no longer appropriate in common gameplay code in 26.2.
# TreeChop only needs block reads in these signatures, so use BlockGetter.
for root in [Path('fabric/src'), Path('shared/src')]:
    for f in root.rglob('*.java'):
        txt=f.read_text()
        txt=txt.replace('net.minecraft.world.level.BlockAndTintGetter', 'net.minecraft.world.level.BlockGetter')
        txt=txt.replace('BlockAndTintGetter', 'BlockGetter')
        f.write_text(txt)

# 26.2 key mappings use a registered KeyMapping.Category instead of a String category.
f=Path('shared/src/main/java/ht/treechop/client/KeyBindings.java')
t=f.read_text()
t=t.replace('import net.minecraft.client.Minecraft;\n', 'import net.minecraft.client.Minecraft;\nimport net.minecraft.resources.Identifier;\n')
t=t.replace('public static final String CATEGORY = "HT\'s TreeChop";', 'public static final KeyMapping.Category CATEGORY = KeyMapping.Category.register(Identifier.fromNamespaceAndPath(TreeChop.MOD_ID, "keys"));')
f.write_text(t)
PY

printf 'Second-stage 26.2 API migrations applied.\n'
