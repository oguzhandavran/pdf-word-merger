# Quick Start Guide - PDF & Word Merger

## Getting Started in 5 Minutes

### Prerequisites
- Windows 10 or Windows 11
- .NET 8.0 Runtime (Download from: https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

**Option 1: Use Pre-compiled Executable**
1. Navigate to `bin/Release/` folder
2. Find `PDFWordMerger.exe` (or just `PDFWordMerger`)
3. Double-click to launch the application

**Option 2: Build from Source**
```bash
cd PDFWordMerger
dotnet restore
dotnet build --configuration Release
```

The compiled executable will be in:
`PDFWordMerger/bin/Release/net8.0-windows/PDFWordMerger.exe`

## Using the Application

### Step 1: Add Your Documents

**Method A: File Browser**
1. Click the **"Add Files"** button
2. Select one or more PDF or Word documents
3. Click "Open"

**Method B: Drag & Drop**
1. Drag files from Windows Explorer
2. Drop them onto the application window
3. Files will be added automatically

### Step 2: Arrange Documents

**Manual Ordering:**
1. Click on a document in the list to select it
2. Use **"↑ Move Up"** or **"↓ Move Down"** buttons to reorder
3. The numbers on the right show the merge order

**Auto-Sort:**
- Click **"📅 Auto-Sort by Date"** to sort by modification date
- Click **"🔤 Auto-Sort by Name"** to sort alphabetically

**Remove Documents:**
- Select a document
- Click **"🗑 Delete"** to remove it

### Step 3: Preview the Merge

1. Click **"📄 Preview Merge"** button
2. Review the merge plan:
   - Document order
   - Page counts
   - Page number ranges
3. Click "OK" to close preview

### Step 4: Export to PDF

1. Click **"💾 Export to PDF"** button
2. Choose save location and filename
3. Click "Save"
4. Wait for the progress bar to complete
5. Success message will show the output location

## Example Walkthrough

### Scenario: Merge 3 reports into one document

1. **Add files:**
   - Drag `Q1_Report.pdf`, `Q2_Report.docx`, `Q3_Report.pdf` onto the window

2. **Check the order:**
   - Files appear in the order they were added
   - Q1 Report → Order 1 (Pages 1-5)
   - Q2 Report → Order 2 (Pages 6-8)
   - Q3 Report → Order 3 (Pages 9-15)

3. **Sort if needed:**
   - Click "📅 Auto-Sort by Date" if you want chronological order

4. **Preview:**
   - Click "📄 Preview Merge"
   - Verify total: 15 pages

5. **Export:**
   - Click "💾 Export to PDF"
   - Save as `Quarterly_Reports_2024.pdf`
   - Done! ✅

## Understanding the Interface

### Left Panel - Document List
Shows your selected files with:
- **Icon**: 📄 (PDF) or 📝 (Word)
- **File name**: Display name
- **File path**: Full location on disk
- **Page count**: Number of pages in document
- **Modified date**: Last modification timestamp
- **Order number**: Position in merge sequence (right side)

### Right Panel - Preview
Shows how your documents will be merged:
- List of all documents in order
- Page count for each document
- Total page count at bottom

### Bottom Section
- **Summary Bar**: Total documents and pages
- **Action Buttons**: Preview and Export
- **Status Bar**: Current operation and progress

## Tips & Tricks

### Efficient Workflow
1. **Create a folder** with all documents to merge
2. **Select all files** in Windows Explorer (Ctrl+A)
3. **Drag them all** to the application at once
4. **Auto-sort** if needed
5. **Export** immediately

### Handling Many Documents
- The app can handle 100+ documents
- Large files may take longer to process
- Watch the progress bar for status

### Best Practices
- **Name files clearly** for easy identification
- **Use modification dates** for chronological ordering
- **Preview before exporting** to verify order
- **Choose descriptive output names** for easy retrieval

### Keyboard Shortcuts
While the app doesn't have built-in shortcuts, you can:
- **Tab** through controls
- **Enter** to activate buttons
- **Ctrl+Click** for multi-select in file dialog
- **Shift+Click** for range select in file dialog

## Common Tasks

### Merge multiple PDF invoices
1. Add all invoice PDFs
2. Auto-sort by date
3. Export as `Invoices_January_2024.pdf`

### Combine meeting notes
1. Add Word and PDF meeting notes
2. Manually arrange by meeting date
3. Export as `Meeting_Notes_Q1.pdf`

### Create a portfolio
1. Add resume (Word), projects (PDF), certifications (PDF)
2. Arrange in desired order
3. Export as `Professional_Portfolio.pdf`

### Compile research papers
1. Add multiple PDF papers
2. Sort alphabetically by name
3. Export as `Research_Collection.pdf`

## Troubleshooting Quick Fixes

### "Can't open the file"
- Check file isn't open in another program
- Verify you have read permission
- Try copying file to another location first

### "Export failed"
- Ensure you have write permission to output folder
- Check available disk space
- Close output file if already open

### "Wrong page count"
- Word files show estimated page count
- Actual count may vary slightly after conversion
- Preview merge to verify

### "Application won't start"
- Install .NET 8.0 Runtime
- Check Windows version (10/11 required)
- Run as Administrator if needed

## Need More Help?

- **Detailed Documentation**: See `README.md`
- **Architecture Details**: See `ARCHITECTURE.md`
- **Issues**: Create an issue in the repository
- **Feature Requests**: Submit via issues with "enhancement" label

## Version Information

- **Current Version**: 1.0
- **Framework**: .NET 8.0
- **Platform**: Windows (WPF)
- **License**: MIT

---

Happy Merging! 🎉
