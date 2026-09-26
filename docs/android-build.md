# Android build and release

Use **Unity 6000.5.9f1**, matching `ProjectSettings/ProjectVersion.txt`, with Android Build Support, SDK/NDK tools and OpenJDK. Keep the package ID unchanged to preserve save compatibility.

## Build on a computer with Unity

1. Open the project with the matching licensed Unity editor.
2. Switch the build target to Android. Configure the existing app's signing keystore in Player Settings if installing this APK as an update. A different key cannot update the installed app.
3. Choose **Castle Master > Build Android APK**. This builds `build/Android/Castle-Master-3D.apk` and runs the offline-shop and fullscreen validation first.

Batch equivalent (after configuring Android signing in the editor):

```sh
Unity -batchmode -nographics -quit -projectPath . -buildTarget Android -executeMethod AndroidBuild.Build -logFile build.log
```

## GitHub Actions

The `Android APK release` workflow runs on `v*` tags, pull requests to `main`, or manually.

- Pull requests and manual runs build a **test APK** using Unity's default Android signing path. They need Unity credentials, but they do **not** receive the release keystore.
- A `v*` tag runs the **release build** inside the GitHub Environment named `release`. Only that job receives the Android release-signing secrets.
- A successful release build publishes the APK and checksum to GitHub Releases.
- The GameCI action is pinned to commit `4423ee75828dff56037d1d1cfdac6b68e3a6ba00`, and its CLI is pinned to `v0.1.69`, so those credential-bearing components do not silently follow moving `v4` / `latest` references.

### Repository Actions secrets

Configure the Unity activation credentials under **Settings → Secrets and variables → Actions**. Never commit them:

- A supported `UNITY_LICENSE` **or** `UNITY_SERIAL`.
- `UNITY_EMAIL`.
- `UNITY_PASSWORD`.

Follow [GameCI activation](https://game.ci/docs/github/activation/). A Unity Personal account alone is not a serial; current Personal licensing can require an already activated local Unity installation instead of hosted CI.

These secrets are needed for PR/manual builds as well as release builds, so they remain repository-level Actions secrets.

### Release environment and signing secrets

Create a GitHub Environment named **`release`** under **Settings → Environments → New environment**.

Store the Android release-signing values as **environment secrets**, not repository-wide secrets:

- `ANDROID_KEYSTORE_BASE64`: base64 contents of the release keystore.
- `ANDROID_KEYSTORE_PASS`.
- `ANDROID_KEYALIAS_NAME`.
- `ANDROID_KEYALIAS_PASS`.

If you previously stored those four values as repository Actions secrets, remove the repository-level copies after adding them to the `release` environment.

For extra protection, configure the `release` environment to require your approval before deployment where your GitHub plan/repository settings support it. That makes possession of write access to a branch insufficient by itself to use the release signing key.

The builder uses the matching Unity Android image. If that editor image is not yet available in GameCI, use the local build above rather than changing the project's editor version blindly.

After configuration, re-run the workflow. For subsequent releases, increase `bundleVersion` and `AndroidBundleVersionCode` in Player Settings, update release notes, commit, and push a matching `v<bundleVersion>` tag.

To publish a locally built APK for an existing tag:

```sh
cd build/Android
sha256sum Castle-Master-3D.apk > Castle-Master-3D.apk.sha256
cd ../..
gh release create v1.0.9 build/Android/Castle-Master-3D.apk build/Android/Castle-Master-3D.apk.sha256 --verify-tag --title 'Castle Master 3D v1.0.9' --notes-file docs/release-notes.md
```

## Device checks before relying on this build

- In airplane mode, claim each gem/gold pack and a command-point capacity upgrade. Close/reopen the shop and restart the game to verify the grant persists. Confirm the existing cap behavior remains.
- Command-point recharge still costs in-game gems; the legacy ad/free-charge feature remains disabled.
- Check title, player selection, world map, castle and battle at 3:2, 16:9 and 20:9. Tap edge buttons and drag the map. Check rotation/resume and camera-cutout placement.
- Fullscreen deliberately stretches the fixed legacy UI to fill the display. It does not crop the layout or alter world-space 3D camera framing. Preview render textures retain their own aspect ratios.
- Signature verification and build-time checks are not a physical-device gameplay test.
