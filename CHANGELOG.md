# 0.0.8 - 2024/07/20

### Bug Fixes
 - Fix issue where clipboard notifications were only working after a user had opened the Clipboard tab
 - Fix issue where some Settings were not being persisted

### Improvements
 - Removed countly

### Codebase Improvements
 - added make commands to generate and view code test coverage reports

# 0.0.7 - 2024/07/15

### Features
 - **My Keys** :key:
   - Added xpubs to Branta, get notified when an address for any added xpub is copied to the clipboard
  
### Improvements
 - UI: New navigation tabs, and reorganized views
   - Settings is now a tab, and can no longer be accessed from the App Drawer icon
   - The Main Window has been split into 'Wallets' and 'Clipboard' tabs
 - Only notify Update available if installed version is older

### Codebase Improvements
 - make use of MVVM Community toolkit
 - use Vertical Slice Architecture to structure app features

# 0.0.6 - 2024/06/19

 - WiX add Startup shortcut on install to run Branta on Startup by default
