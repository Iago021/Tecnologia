#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

python3 - <<'PY'
from pathlib import Path
import json

# The original chopped_log model is a placeholder consumed by TreeChop's pre-26.2
# custom model loader. That renderer was removed from the core port, so provide a
# vanilla fallback model to avoid a missing/recursive model at runtime.
model = Path('shared/src/main/resources/assets/treechop/models/block/chopped_log.json')
if not model.exists():
    model = Path('fabric/src/main/resources/assets/treechop/models/block/chopped_log.json')
model.parent.mkdir(parents=True, exist_ok=True)
model.write_text(json.dumps({
    'parent': 'minecraft:block/cube_column',
    'textures': {
        'side': 'minecraft:block/oak_log',
        'end': 'minecraft:block/oak_log_top'
    }
}, indent=2) + '\n')

# Run all surviving mixins at the same Java level as Minecraft 26.2.
for f in list(Path('shared/src/main/resources').rglob('*mixins.json')) + list(Path('fabric/src/main/resources').rglob('*mixins.json')):
    try:
        data=json.loads(f.read_text())
    except Exception:
        continue
    data['compatibilityLevel']='JAVA_25'
    f.write_text(json.dumps(data, indent=2)+'\n')
PY

printf 'Runtime resource safety patch applied.\n'
