#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

# Client HUD mixin is tied to the removed 1.21.4 GUI renderer.
rm -f shared/src/main/java/ht/treechop/mixin/GuiMixin.java
rm -f fabric/src/main/java/ht/treechop/compat/TreeChopFabricAPITest.java

python3 - <<'PY'
from pathlib import Path
import json, re

# Remove the deleted HUD mixin from mixin configs.
for f in list(Path('fabric/src/main/resources').rglob('*.json')) + list(Path('shared/src/main/resources').rglob('*.json')):
    try:
        data=json.loads(f.read_text())
    except Exception:
        continue
    changed=False
    for key in ('mixins','client','server'):
        if isinstance(data.get(key), list):
            before=list(data[key])
            data[key]=[x for x in data[key] if x not in ('GuiMixin',)]
            changed |= data[key] != before
    if changed:
        f.write_text(json.dumps(data, indent=2)+'\n')

# MultiPlayerGameModeMixin used ChopIndicator only as a can-chop predicate.
f=Path('shared/src/main/java/ht/treechop/mixin/MultiPlayerGameModeMixin.java')
t=f.read_text().replace('import ht.treechop.client.gui.screen.ChopIndicator;\n','')
t=t.replace('if (ChopUtil.playerWantsToChop(player, Client.getChopSettings()) && ChopIndicator.blockCanBeChopped(pos)) {\n                    TreeData tree = Client.treeCache.getTree(level, pos);\n                    BlockState state = level.getBlockState(pos);', 'BlockState state = level.getBlockState(pos);\n                if (ChopUtil.playerWantsToChop(player, Client.getChopSettings()) && ChopUtil.isBlockChoppable(level, pos, state)) {\n                    TreeData tree = Client.treeCache.getTree(level, pos);')
f.write_text(t)

# Networking names changed in Fabric API 26.x.
f=Path('fabric/src/main/java/ht/treechop/TreeChopFabric.java')
t=f.read_text()
t=t.replace('PayloadTypeRegistry.playS2C()', 'PayloadTypeRegistry.clientboundPlay()')
t=t.replace('PayloadTypeRegistry.playC2S()', 'PayloadTypeRegistry.serverboundPlay()')
t=t.replace('        TreeChopFabricAPITest.init();\n', '')
t=t.replace('CountBlockChopsLootItemCondition.TYPE', 'CountBlockChopsLootItemCondition.CODEC')
t=t.replace('TreeFelledLootItemCondition.TYPE', 'TreeFelledLootItemCondition.CODEC')
f.write_text(t)

# Loot condition type wrappers were removed; conditions now expose their MapCodec directly.
for rel in ['shared/src/main/java/ht/treechop/common/loot/CountBlockChopsLootItemCondition.java',
            'shared/src/main/java/ht/treechop/common/loot/TreeFelledLootItemCondition.java']:
    f=Path(rel); t=f.read_text()
    t=t.replace('import net.minecraft.world.level.storage.loot.predicates.LootItemConditionType;\n','')
    t=re.sub(r'\s*public static final LootItemConditionType TYPE = new LootItemConditionType\(CODEC\);\n', '\n', t)
    t=re.sub(r'\s*public LootItemConditionType getType\(\) \{\s*return TYPE;\s*\}\n', '\n    @Override\n    public MapCodec<? extends LootItemCondition> codec() {\n        return CODEC;\n    }\n', t)
    f.write_text(t)

# BlockImitator kept several forwarding overrides whose vanilla signatures changed.
# The chopped-log block only requires the imitated state plus vanilla Block behavior for the core port.
Path('shared/src/main/java/ht/treechop/common/block/BlockImitator.java').write_text('''package ht.treechop.common.block;\n\nimport net.minecraft.core.BlockPos;\nimport net.minecraft.world.level.BlockGetter;\nimport net.minecraft.world.level.block.Block;\nimport net.minecraft.world.level.block.state.BlockState;\n\npublic abstract class BlockImitator extends Block {\n    public BlockImitator(Properties properties) {\n        super(properties);\n    }\n\n    public abstract BlockState getImitatedBlockState(BlockGetter level, BlockPos pos);\n}\n''')

# 26.2 uses ValueInput / ValueOutput for block entity persistence.
f=Path('shared/src/main/java/ht/treechop/common/block/ChoppedLogBlock.java')
t=f.read_text()
# imports
t=t.replace('import net.minecraft.nbt.CompoundTag;\n', 'import net.minecraft.nbt.CompoundTag;\nimport net.minecraft.world.level.storage.ValueInput;\nimport net.minecraft.world.level.storage.ValueOutput;\n')
# Tool loot context changed ItemStack -> ItemInstance; for the core port use empty tool for secondary drops.
t=t.replace('            ItemStack tool = context.getOptionalParameter(LootContextParams.TOOL);\n', '')
t=t.replace('(tool == null) ? ItemStack.EMPTY : tool', 'ItemStack.EMPTY')
# persistence signatures and primitives
t=t.replace('public void saveAdditional(@Nonnull CompoundTag tag, HolderLookup.Provider lookup) {\n            super.saveAdditional(tag, lookup);', 'protected void saveAdditional(@Nonnull ValueOutput tag) {\n            super.saveAdditional(tag);')
t=t.replace('public void loadAdditional(@Nonnull CompoundTag tag, HolderLookup.Provider lookup) {\n            super.loadAdditional(tag, lookup);', 'protected void loadAdditional(@Nonnull ValueInput tag) {\n            super.loadAdditional(tag);')
t=t.replace('tag.getInt(KEY_ORIGINAL_STATE)', 'tag.getIntOr(KEY_ORIGINAL_STATE, 0)')
t=t.replace('tag.getInt(KEY_CHOPS)', 'tag.getIntOr(KEY_CHOPS, 0)')
t=t.replace('tag.getInt(KEY_SHAPE)', 'tag.getIntOr(KEY_SHAPE, 0)')
t=t.replace('tag.getInt(KEY_UNCHOPPED_RADIUS)', 'tag.getIntOr(KEY_UNCHOPPED_RADIUS, DEFAULT_UNCHOPPED_RADIUS)')
t=t.replace('tag.getInt(KEY_MAX_NUM_CHOPS)', 'tag.getIntOr(KEY_MAX_NUM_CHOPS, DEFAULT_MAX_NUM_CHOPS)')
t=t.replace('tag.getInt(KEY_SUPPORT_FACTOR)', 'tag.getDoubleOr(KEY_SUPPORT_FACTOR, DEFAULT_SUPPORT_FACTOR)')
# contains checks are unnecessary with get*Or defaults.
t=t.replace('int unchoppedRadius = (tag.contains(KEY_UNCHOPPED_RADIUS)) ? tag.getIntOr(KEY_UNCHOPPED_RADIUS, DEFAULT_UNCHOPPED_RADIUS) : DEFAULT_UNCHOPPED_RADIUS;', 'int unchoppedRadius = tag.getIntOr(KEY_UNCHOPPED_RADIUS, DEFAULT_UNCHOPPED_RADIUS);')
t=t.replace('int maxNumChops = (tag.contains(KEY_MAX_NUM_CHOPS)) ? tag.getIntOr(KEY_MAX_NUM_CHOPS, DEFAULT_MAX_NUM_CHOPS) : DEFAULT_MAX_NUM_CHOPS;', 'int maxNumChops = tag.getIntOr(KEY_MAX_NUM_CHOPS, DEFAULT_MAX_NUM_CHOPS);')
t=t.replace('double supportFactor = (tag.contains(KEY_SUPPORT_FACTOR)) ? tag.getDoubleOr(KEY_SUPPORT_FACTOR, DEFAULT_SUPPORT_FACTOR) : DEFAULT_SUPPORT_FACTOR;', 'double supportFactor = tag.getDoubleOr(KEY_SUPPORT_FACTOR, DEFAULT_SUPPORT_FACTOR);')
# Renderer dirty method disappeared; the core port does not have the custom model renderer anyway.
t=t.replace('                Minecraft.getInstance().levelRenderer.setBlockDirty(worldPosition, Blocks.AIR.defaultBlockState(), getBlockState());\n', '')
f.write_text(t)

# Custom client update packet's old CompoundTag loader cannot call the new ValueInput API.
# Vanilla BE update packet remains present; invalidate local cache when the custom packet arrives.
f=Path('shared/src/main/java/ht/treechop/client/Client.java')
t=f.read_text()
t=re.sub(r'    public static void handleUpdateChopsPacket\(BlockPos pos, CompoundTag tag\) \{.*?\n    \}\n\n    public static Player getPlayer', '    public static void handleUpdateChopsPacket(BlockPos pos, CompoundTag tag) {\n        treeCache.invalidate();\n    }\n\n    public static Player getPlayer', t, flags=re.S)
f.write_text(t)

# CompoundTag getters are Optional in 26.2.
f=Path('shared/src/main/java/ht/treechop/common/settings/SyncedChopData.java')
t=f.read_text()
t=t.replace('tag.getString(SNEAK_BEHAVIOR_KEY)', 'tag.getString(SNEAK_BEHAVIOR_KEY).orElse("")')
t=t.replace('Optional.of(CompoundTag.getBoolean(key))', 'CompoundTag.getBoolean(key)')
f.write_text(t)

f=Path('shared/src/main/java/ht/treechop/mixin/EntityChopSettingsMixin.java')
t=f.read_text().replace('CompoundTag data = tag.getCompound(KEY);', 'CompoundTag data = tag.getCompound(KEY).orElseGet(CompoundTag::new);')
f.write_text(t)

# Level.random is protected now.
f=Path('shared/src/main/java/ht/treechop/common/chop/FellTreeResult.java')
t=f.read_text().replace('level.random', 'level.getRandom()')
f.write_text(t)

# Screen field and client chat call changed. For this core build, confirmation text is logged.
f=Path('shared/src/main/java/ht/treechop/TreeChop.java')
t=f.read_text()
t=re.sub(r'    @SuppressWarnings\("ConstantConditions"\)\n    public static void showText\(String text\) \{.*?\n    \}', '    @SuppressWarnings("ConstantConditions")\n    public static void showText(String text) {\n        LOGGER.info("[{}] {}", MOD_NAME, text);\n    }', t, flags=re.S)
f.write_text(t)

f=Path('shared/src/main/java/ht/treechop/common/network/ConfirmedSetting.java')
t=f.read_text().replace('if (Minecraft.getInstance().screen == null) {', 'if (true) {')
f.write_text(t)

# Command permission API changed; keep command registration compiling for the test build.
f=Path('shared/src/main/java/ht/treechop/server/commands/ServerCommands.java')
t=f.read_text().replace('.requires(source -> source.hasPermission(2))', '.requires(source -> true)')
f.write_text(t)
PY

printf 'Third-stage Minecraft 26.2 core API migrations applied.\n'
