# BooruGod - Auto-Updater System

## How the Auto-Updater Works

The BooruGod app includes an automatic update system that checks for new versions when the app starts.

### Current Setup

- **GitHub Repository**: https://github.com/Serial-Zero/BooruGod
- **Version File**: https://raw.githubusercontent.com/Serial-Zero/BooruGod/main/version.json
- **Releases Page**: https://github.com/Serial-Zero/BooruGod/releases

### How to Release a New Version

1. **Build your APK**:
   ```bash
   dotnet build -f net9.0-android
   ```

2. **Create a GitHub Release**:
   - Go to https://github.com/Serial-Zero/BooruGod/releases
   - Click "Create a new release"
   - Tag: `v1.0.1` (increment version number)
   - Title: `BooruGod v1.0.1`
   - Description: Add your release notes
   - Upload the APK file

3. **Update version.json**:
   ```json
   {
     "version": "1.0.1",
     "downloadUrl": "https://github.com/Serial-Zero/BooruGod/releases/download/v1.0.1/BooruGod.apk",
     "releaseNotes": "Fixed video sync issues, improved performance",
     "mandatory": false,
     "minVersion": "1.0.0"
   }
   ```

4. **Commit and push**:
   ```bash
   git add version.json
   git commit -m "Update to version 1.0.1"
   git push origin main
   ```

### Version.json Fields

- **version**: The new version number (e.g., "1.0.1")
- **downloadUrl**: Direct link to the APK file in GitHub releases
- **releaseNotes**: Description of changes for users
- **mandatory**: Set to `true` if users must update
- **minVersion**: Minimum supported version

### How Users Get Updates

1. When users open the app, it automatically checks for updates
2. If a new version is available, they see an update dialog
3. They can click "Download Update" to go to the GitHub release
4. They download and install the new APK manually

### Testing the Auto-Updater

1. Update version.json with a higher version number
2. Push to GitHub
3. Open the app - you should see the update dialog

### Troubleshooting

- **No update dialog**: Check that version.json is accessible at the GitHub raw URL
- **Download fails**: Ensure the APK file is uploaded to the GitHub release
- **Version comparison issues**: Make sure version numbers follow semantic versioning (e.g., "1.0.1")
