#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

cat > gradle.properties <<'EOF'
# Mod
mod_id=treechop
mod_group=ht.treechop
mod_version=0.19.4+26.2-port.2
mod_authors=hammertater
mod_name=TreeChop
mod_name_gloating=HT's TreeChop
mod_description=Makes trees more choppable.
mod_credits=This one goes out to all the lumberjacks.
mod_license=MIT (https://github.com/hammertater/treechop/blob/main/LICENSE)
mod_url_curseforge=https://www.curseforge.com/minecraft/mc-mods/treechop
mod_url_modrinth=https://modrinth.com/mod/treechop
mod_url_source=https://github.com/hammertater/treechop
mod_url_issues=https://github.com/hammertater/treechop/issues

# Minecraft 26.2 / Fabric
minecraft_version=26.2
minecraft_version_range=[26.2,26.3)
fabric_api_version=0.156.0+26.2
fabric_loader_version=0.19.3
forgeconfigapiport_version=26.2.1
loom_version=1.17-SNAPSHOT

# Build
org.gradle.jvmargs=-Xmx4G
org.gradle.daemon=false
org.gradle.parallel=true
org.gradle.configuration-cache=false
java_version=25
EOF

cat > settings.gradle <<'EOF'
pluginManagement {
    repositories {
        maven { url = 'https://maven.fabricmc.net/' }
        mavenCentral()
        gradlePluginPortal()
        maven { url = 'https://repo.spongepowered.org/repository/maven-public/' }
    }
}
rootProject.name = mod_name
include 'tuber', 'shared', 'fabric'
EOF

cat > build.gradle <<'EOF'
plugins {
    id 'net.fabricmc.fabric-loom' version "$loom_version" apply false
    id 'org.spongepowered.mixin' version '0.7-SNAPSHOT' apply false
}

subprojects {
    apply plugin: 'java'
    apply plugin: 'idea'
    apply plugin: 'maven-publish'

    java.toolchain.languageVersion = JavaLanguageVersion.of(25)

    repositories {
        mavenCentral()
        maven { url = 'https://maven.fabricmc.net' }
        maven { url = 'https://maven.terraformersmc.com/' }
        maven { url = 'https://maven.bai.lol' }
        maven { url = 'https://raw.githubusercontent.com/Fuzss/modresources/main/maven/' }
        maven { url = 'https://api.modrinth.com/maven' }
        maven { url = 'https://repo.spongepowered.org/repository/maven-public/' }
        maven { url = 'https://www.cursemaven.com' }
    }

    dependencies {
        implementation 'com.google.code.findbugs:jsr305:3.0.2'
    }

    tasks.withType(JavaCompile).configureEach {
        options.encoding = 'UTF-8'
        options.release = 25
    }
}
EOF

cat > shared/build.gradle <<'EOF'
dependencies {
    compileOnly 'org.spongepowered:mixin:0.8.7'
    compileOnly project(':tuber')
}
EOF

python3 - <<'PY'
from pathlib import Path
import json, re

# Modernize Fabric Gradle dependency configuration.
p=Path('fabric/build.gradle')
s=p.read_text()
s=s.replace('plugins {\n    id "fabric-loom"\n}', 'plugins {\n    id "net.fabricmc.fabric-loom"\n}')
s=s.replace('    mappings loom.layered() {\n        officialMojangMappings()\n        parchment("org.parchmentmc.data:parchment-$minecraft_version:$parchment_mappings_version@zip")\n    }\n', '')
s=s.replace('    implementation project(":shared")\n', '')
for line in [
    '    modApi "com.terraformersmc:modmenu:13.0.4"\n',
    '    modCompileOnly "mcp.mobius.waila:wthit-api:fabric-14.6.2"\n',
    '    modRuntimeOnly "mcp.mobius.waila:wthit:fabric-14.6.2"\n',
    '    modRuntimeOnly "lol.bai:badpackets:fabric-0.8.2"\n',
    '    modCompileOnly "curse.maven:jade-324717:6155088"\n',
    '    modCompileOnly "com.terraformersmc.terraform-api:terraform-wood-api-v1:13.0.0"\n',
]:
    s=s.replace(line, '')
s=s.replace('modImplementation ', 'implementation ')
s=s.replace('modApi ', 'implementation ')
s=s.replace('modCompileOnly ', 'compileOnly ')
s=s.replace('modRuntimeOnly ', 'runtimeOnly ')
# First functional port: do not compile optional integrations or the pre-26.2 custom model renderer.
needle='sourceSets {\n    main {\n        java {'
replacement='sourceSets {\n    main {\n        java {\n            exclude "ht/treechop/compat/Jade.java"\n            exclude "ht/treechop/compat/Wthit.java"\n            exclude "ht/treechop/compat/Terraformers.java"\n            exclude "ht/treechop/compat/ModMenu.java"\n            exclude "ht/treechop/client/model/**"'
s=s.replace(needle, replacement)
p.write_text(s)

# Minecraft 26.x Mojang mapping rename: ResourceLocation -> Identifier.
for root in [Path('fabric/src'), Path('shared/src'), Path('tuber/src')]:
    for f in root.rglob('*.java'):
        txt=f.read_text()
        txt=txt.replace('net.minecraft.resources.ResourceLocation', 'net.minecraft.resources.Identifier')
        txt=txt.replace('ResourceLocation.fromNamespaceAndPath', 'Identifier.fromNamespaceAndPath')
        txt=txt.replace('ResourceLocation.parse', 'Identifier.parse')
        txt=txt.replace('ResourceLocation.withDefaultNamespace', 'Identifier.withDefaultNamespace')
        txt=re.sub(r'new\s+ResourceLocation\s*\(([^,\n]+),\s*([^\)\n]+)\)', r'Identifier.fromNamespaceAndPath(\1, \2)', txt)
        txt=txt.replace('ResourceLocation', 'Identifier')
        f.write_text(txt)

# Forge Config API Port 26.2 moved Fabric API to v5.
f=Path('fabric/src/main/java/ht/treechop/TreeChopFabric.java')
t=f.read_text()
t=t.replace('fuzs.forgeconfigapiport.fabric.api.neoforge.v4.NeoForgeConfigRegistry', 'fuzs.forgeconfigapiport.fabric.api.v5.ConfigRegistry')
t=t.replace('fuzs.forgeconfigapiport.fabric.api.neoforge.v4.NeoForgeModConfigEvents', 'fuzs.forgeconfigapiport.fabric.api.v5.ModConfigEvents')
t=t.replace('NeoForgeModConfigEvents.', 'ModConfigEvents.')
t=t.replace('NeoForgeConfigRegistry.INSTANCE', 'ConfigRegistry.INSTANCE')
t=t.replace('import ht.treechop.compat.TreeChopFabricAPITest;\n', '')
t=t.replace('        TreeChopFabricAPITest.init();\n', '')
f.write_text(t)

# Minimal 26.2 client initializer. Gameplay/network/keybinds stay enabled while renderer is ported separately.
Path('fabric/src/main/java/ht/treechop/client/FabricClient.java').write_text('''package ht.treechop.client;\n\nimport ht.treechop.common.network.ServerConfirmSettingsPacket;\nimport ht.treechop.common.network.ServerPermissionsPacket;\nimport ht.treechop.common.network.ServerUpdateChopsPacket;\nimport net.fabricmc.api.ClientModInitializer;\nimport net.fabricmc.api.EnvType;\nimport net.fabricmc.api.Environment;\nimport net.fabricmc.fabric.api.client.event.lifecycle.v1.ClientTickEvents;\nimport net.fabricmc.fabric.api.client.keymapping.v1.KeyMappingHelper;\nimport net.fabricmc.fabric.api.client.networking.v1.ClientPlayConnectionEvents;\nimport net.fabricmc.fabric.api.client.networking.v1.ClientPlayNetworking;\nimport net.minecraft.network.protocol.common.custom.CustomPacketPayload;\n\n@Environment(EnvType.CLIENT)\npublic class FabricClient extends Client implements ClientModInitializer {\n    static { Client.instance = new FabricClient(); }\n\n    @Override\n    public void onInitializeClient() {\n        ClientPlayConnectionEvents.JOIN.register((handler, sender, client) -> syncOnJoin());\n        registerPackets();\n        registerKeybindings();\n    }\n\n    private void registerKeybindings() {\n        KeyBindings.registerKeyMappings(KeyMappingHelper::registerKeyMapping);\n        ClientTickEvents.END_CLIENT_TICK.register(client -> {\n            for (KeyBindings.ActionableKeyBinding keyBinding : KeyBindings.allKeyBindings) {\n                if (keyBinding.consumeClick()) {\n                    keyBinding.onPress();\n                    return;\n                }\n            }\n        });\n    }\n\n    private void registerPackets() {\n        ClientPlayNetworking.registerGlobalReceiver(ServerConfirmSettingsPacket.TYPE, (payload, context) -> payload.handle());\n        ClientPlayNetworking.registerGlobalReceiver(ServerPermissionsPacket.TYPE, (payload, context) -> payload.handle());\n        ClientPlayNetworking.registerGlobalReceiver(ServerUpdateChopsPacket.TYPE, (payload, context) -> payload.handle());\n    }\n\n    @Override\n    public void sendToServer(CustomPacketPayload payload) {\n        ClientPlayNetworking.send(payload);\n    }\n}\n''')

# Remove optional entrypoints that are intentionally excluded in this first 26.2 build.
mod=Path('fabric/src/main/resources/fabric.mod.json')
data=json.loads(mod.read_text())
for k in ['jade','modmenu']:
    data.get('entrypoints', {}).pop(k, None)
data['depends']['java']='>=25'
mod.write_text(json.dumps(data, indent=2) + '\n')
PY

(grep -RIl 'JAVA_21' fabric/src shared/src 2>/dev/null || true) | xargs -r sed -i 's/JAVA_21/JAVA_25/g'
(grep -RIl '1.21.4' fabric/src/main/resources 2>/dev/null || true) | xargs -r sed -i 's/1\.21\.4/26.2/g'
sed -i 's#distributionUrl=.*#distributionUrl=https\\://services.gradle.org/distributions/gradle-9.5.1-bin.zip#' gradle/wrapper/gradle-wrapper.properties

printf '\nTreeChop 26.2 bootstrap + source port applied.\n'
