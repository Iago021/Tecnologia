#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-treechop}"
cd "$ROOT"

cat > gradle.properties <<'EOF'
# Mod
mod_id=treechop
mod_group=ht.treechop
mod_version=0.19.4+26.2-port.1
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
    id 'org.spongepowered.gradle.vanilla' version '0.2.1-SNAPSHOT' apply false
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
plugins {
    id 'org.spongepowered.gradle.vanilla'
}

dependencies {
    compileOnly 'org.spongepowered:mixin:0.8.7'
    compileOnly "fuzs.forgeconfigapiport:forgeconfigapiport-common:$forgeconfigapiport_version"
    compileOnly project(':tuber')
}

minecraft {
    version(minecraft_version)
}
EOF

python3 - <<'PY'
from pathlib import Path
p=Path('fabric/build.gradle')
s=p.read_text()
s=s.replace('plugins {\n    id "fabric-loom"\n}', 'plugins {\n    id "net.fabricmc.fabric-loom"\n}')
s=s.replace('    mappings loom.layered() {\n        officialMojangMappings()\n        parchment("org.parchmentmc.data:parchment-$minecraft_version:$parchment_mappings_version@zip")\n    }\n', '')
s=s.replace('    modApi "com.terraformersmc:modmenu:13.0.4"\n', '')
s=s.replace('    modCompileOnly "mcp.mobius.waila:wthit-api:fabric-14.6.2"\n', '')
s=s.replace('    modRuntimeOnly "mcp.mobius.waila:wthit:fabric-14.6.2"\n', '')
s=s.replace('    modRuntimeOnly "lol.bai:badpackets:fabric-0.8.2"\n', '')
s=s.replace('    modCompileOnly "curse.maven:jade-324717:6155088"\n', '')
s=s.replace('    modCompileOnly "com.terraformersmc.terraform-api:terraform-wood-api-v1:13.0.0"\n', '')
p.write_text(s)
PY

# 26.2 requires Java 25 in mixin configs and fabric.mod.json.
(grep -RIl 'JAVA_21' fabric/src shared/src 2>/dev/null || true) | xargs -r sed -i 's/JAVA_21/JAVA_25/g'
(grep -RIl '>=21' fabric/src/main/resources 2>/dev/null || true) | xargs -r sed -i 's/">=21"/">=25"/g'
(grep -RIl '1.21.4' fabric/src/main/resources 2>/dev/null || true) | xargs -r sed -i 's/1\.21\.4/26.2/g'

# Use Gradle 9.5.1, matching the current Fabric 26.2 toolchain.
sed -i 's#distributionUrl=.*#distributionUrl=https\\://services.gradle.org/distributions/gradle-9.5.1-bin.zip#' gradle/wrapper/gradle-wrapper.properties

printf '\nBootstrap patch applied for Minecraft 26.2 Fabric.\n'
