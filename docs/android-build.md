# Android build and release

Use **Unity 6000.5.9f1**, matching `ProjectSettings/ProjectVersion.txt`, with Android Build Support, SDK/NDK tools and OpenJDK. Keep the package ID unchanged to preserve save compatibility.

## Build on a computer with Unity

1. Open the project with the matching licensed Unity editor.
2. Switch the build target to Android. Configure the existing app's signing keystore in Player Settings if installing this APK as an update. A different key cannot update the installed app.
3. Choose **Castle Master > Build Android APK**. This builds `build/Android/Castle-Master-3D.apk` and runs the offline-shop and fullscreen validation first.

Batch equivalent:

```sh
Unity -batchmode -nographics -quit -projectPath . -buildTarget Android -executeMethod AndroidBuild.Build -logFile build.log
```

## GitHub Actions

The `Android APK release` workflow runs on `v*` tags, pull requests to `main`, or manually.

The GitHub-hosted runner now:

1. Installs the Unity version from `ProjectSettings/ProjectVersion.txt`.
2. Installs Android Build Support.
3. Activates Unity Personal using your Unity ID credentials.
4. Runs `AndroidBuild.Build`.
5. Verifies the APK signature and creates a SHA-256 checksum.
6. On a `v*` tag, publishes the APK to GitHub Releases.

The credential-bearing third-party actions are pinned to exact commits:

- `buildalon/unity-setup` v2.6.0: `30fcbcb56c10ea5d64298e970d952b8d29bc268b`
- `buildalon/activate-unity-license` v2.2.2: `e0d245d0787b7b9931b56ccbde3b508f6b70f1af`
- `buildalon/unity-action` v3.1.0: `2d420bea0f47fbe01377601fa3231bcf4de04f3a`

### Repository Actions secrets

Go to **Settings → Secrets and variables → Actions** and add:

- `UNITY_EMAIL`: your Unity ID email.
- `UNITY_PASSWORD`: your Unity ID password.

For this Unity Personal workflow you do **not** need:

- `UNITY_LICENSE`
- `UNITY_SERIAL`
- a local `.ulf` file

### Release environment and Android signing

Create a GitHub Environment named **`release`** under **Settings → Environments**.

Add these environment secrets:

- `ANDROID_KEYSTORE_BASE64`: the complete Base64-encoded contents of your `release.keystore`.
- `ANDROID_KEYSTORE_PASS`: the keystore password you entered when creating it.
- `ANDROID_KEYALIAS_NAME`: the alias used when creating the key, for example `castle-master`.
- `ANDROID_KEYALIAS_PASS`: the password for that alias/key.

The workflow decodes the keystore only into the GitHub runner's temporary directory. `AndroidBuild.Build` reads these values from environment variables and configures release signing only inside GitHub Actions. The temporary keystore is deleted afterward.

Manual and pull-request builds do not receive the release keystore and use Android debug signing instead.

For extra protection, you can configure the `release` environment to require approval before the release job can access its secrets.

## Create a release

The release tag must match the Unity `bundleVersion`.

The current project version is `1.0.9`, so the matching tag is:

```sh
git tag v1.0.9
git push origin v1.0.9
```

A successful tag build publishes:

- `Castle-Master-3D.apk`
- `Castle-Master-3D.apk.sha256`

to GitHub Releases.

Before later releases, increment both `bundleVersion` and `AndroidBundleVersionCode`, update `docs/release-notes.md`, commit, and then push the matching tag.

## Device checks before relying on this build

- In airplane mode, claim each gem/gold pack and a command-point capacity upgrade. Close/reopen the shop and restart the game to verify the grant persists. Confirm the existing cap behavior remains.
- Command-point recharge still costs in-game gems; the legacy ad/free-charge feature remains disabled.
- Check title, player selection, world map, castle and battle at 3:2, 16:9 and 20:9. Tap edge buttons and drag the map. Check rotation/resume and camera-cutout placement.
- Fullscreen deliberately stretches the fixed legacy UI to fill the display. It does not crop the layout or alter world-space 3D camera framing. Preview render textures retain their own aspect ratios.
- Signature verification and build-time checks are not a physical-device gameplay test.
