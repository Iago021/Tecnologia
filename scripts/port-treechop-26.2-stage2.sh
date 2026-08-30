#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

# These integrations/render paths target the pre-26.2 client APIs. Remove them from
# the first core build so gameplay/networking can be ported independently.
rm -rf fabric/src/main/java/ht/treechop/client/model
rm -rf shared/src/main/java/ht/treechop/client/model
rm -rf shared/src/main/java/ht/treechop/client/gui
rm -f shared/src/main/java/ht/treechop/client/KeyBindings.java
rm -f fabric/src/main/java/ht/treechop/compat/Jade.java
rm -f fabric/src/main/java/ht/treechop/compat/Wthit.java
rm -f fabric/src/main/java/ht/treechop/compat/Terraformers.java
rm -f fabric/src/main/java/ht/treechop/compat/ModMenu.java

python3 - <<'PY'
from pathlib import Path

# BlockAndTintGetter was removed from this gameplay-facing path in 26.2.
for root in [Path('fabric/src'), Path('shared/src')]:
    for f in root.rglob('*.java'):
        txt=f.read_text()
        txt=txt.replace('net.minecraft.world.level.BlockAndTintGetter', 'net.minecraft.world.level.BlockGetter')
        txt=txt.replace('BlockAndTintGetter', 'BlockGetter')
        f.write_text(txt)

# Keep Client's networking/settings state, but remove the old GUI overlay dependency.
f=Path('shared/src/main/java/ht/treechop/client/Client.java')
t=f.read_text()
t=t.replace('import ht.treechop.client.gui.screen.ClientSettingsScreen;\n', '')
start='''    public static void toggleSettingsOverlay() {\n        Minecraft minecraft = Minecraft.getInstance();\n        if (minecraft.screen instanceof ClientSettingsScreen) {\n            minecraft.screen.onClose();\n        } else {\n            minecraft.setScreen(new ClientSettingsScreen());\n        }\n    }\n'''
t=t.replace(start, '''    public static void toggleSettingsOverlay() {\n        // The 1.21.4 GUI renderer was removed in Minecraft 26.2.\n        // Settings remain available through the config file for this core port.\n    }\n''')
f.write_text(t)

# Minimal 26.2 client bootstrap: packet sync only. Key mappings/UI can be restored
# after the core gameplay build is stable.
Path('fabric/src/main/java/ht/treechop/client/FabricClient.java').write_text('''package ht.treechop.client;\n\nimport ht.treechop.common.network.ServerConfirmSettingsPacket;\nimport ht.treechop.common.network.ServerPermissionsPacket;\nimport ht.treechop.common.network.ServerUpdateChopsPacket;\nimport net.fabricmc.api.ClientModInitializer;\nimport net.fabricmc.api.EnvType;\nimport net.fabricmc.api.Environment;\nimport net.fabricmc.fabric.api.client.networking.v1.ClientPlayConnectionEvents;\nimport net.fabricmc.fabric.api.client.networking.v1.ClientPlayNetworking;\nimport net.minecraft.network.protocol.common.custom.CustomPacketPayload;\n\n@Environment(EnvType.CLIENT)\npublic class FabricClient extends Client implements ClientModInitializer {\n    static { Client.instance = new FabricClient(); }\n\n    @Override\n    public void onInitializeClient() {\n        ClientPlayConnectionEvents.JOIN.register((handler, sender, client) -> syncOnJoin());\n        ClientPlayNetworking.registerGlobalReceiver(ServerConfirmSettingsPacket.TYPE, (payload, context) -> payload.handle());\n        ClientPlayNetworking.registerGlobalReceiver(ServerPermissionsPacket.TYPE, (payload, context) -> payload.handle());\n        ClientPlayNetworking.registerGlobalReceiver(ServerUpdateChopsPacket.TYPE, (payload, context) -> payload.handle());\n    }\n\n    @Override\n    public void sendToServer(CustomPacketPayload payload) {\n        ClientPlayNetworking.send(payload);\n    }\n}\n''')
PY

printf 'Second-stage 26.2 API migrations and core-client pruning applied.\n'
