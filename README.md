# PDF & Word Document Merger

A professional WPF desktop application for merging PDF and Word documents with automatic sorting and sequential page numbering.

## Features

### 🎯 Core Functionality
- **Multi-Format Support**: Merge PDF (.pdf) and Word (.doc, .docx) documents seamlessly
- **Drag & Drop**: Intuitive drag-and-drop interface for adding files
- **File Browser**: Standard file selection dialog with multi-select support
- **Document Preview**: Real-time preview of merge order and page counts

### 📋 File Management
- **Reorder Widget**: Move documents up/down with visual indicators
- **Delete Files**: Remove unwanted documents from the merge list
- **File Type Icons**: Visual indicators (📄 PDF, 📝 Word) for quick identification
- **Document Information**: View page count, modification date, and file path

### 🔄 Auto-Sort Options
- **Sort by Date**: Automatically order documents by modification date
- **Sort by Name**: Alphabetically sort documents by filename
- **Manual Override**: Full control to arrange documents in any order

### 📄 Page Management
- **Page Count Detection**: Automatic page counting for all document types
- **Sequential Numbering**: Add page numbers to all pages (1, 2, 3, ...)
- **Footer Placement**: Professional centered page numbers at bottom of each page

### 💾 Export & Output
- **Single PDF Output**: Merge all documents into one PDF file
- **Custom Save Location**: Choose output filename and directory
- **Progress Tracking**: Real-time progress indicator during merge operations
- **Error Handling**: User-friendly error messages and recovery

## Technical Stack

### Framework & Architecture
- **.NET 8.0**: Latest .NET framework with Windows support
- **WPF (Windows Presentation Foundation)**: Rich desktop UI
- **MVVM Pattern**: Clean separation of concerns
  - Models: `DocumentFile`, `MergeOperation`
  - ViewModels: `MainWindowViewModel`, `RelayCommand`
  - Views: `MainWindow.xaml`
  - Services: `FileService`, `MergeService`, `PdfService`, `WordService`

### NuGet Packages
- **iText7** (v8.0.5): PDF manipulation and generation
- **iText7.bouncy-castle-adapter** (v8.0.5): Security and encryption support
- **DocumentFormat.OpenXml** (v3.1.0): Word document parsing and reading

## Project Structure

```
pdf-word-merger/
├── .gitignore                          # Git ignore file (allows .exe)
├── README.md                           # This file
├── PDFWordMerger.sln                   # Visual Studio solution
├── PDFWordMerger/
│   ├── PDFWordMerger.csproj           # Project file
│   ├── App.xaml                        # Application resources
│   ├── App.xaml.cs                     # Application code-behind
│   ├── MainWindow.xaml                 # Main UI layout
│   ├── MainWindow.xaml.cs              # Main window code-behind
│   ├── Converters/
│   │   └── BoolToVisibilityConverter.cs
│   ├── Models/
│   │   ├── DocumentFile.cs             # Document file model
│   │   └── MergeOperation.cs           # Merge operation model
│   ├── ViewModels/
│   │   ├── MainWindowViewModel.cs      # Main view model
│   │   └── RelayCommand.cs             # Command implementation
│   └── Services/
│       ├── FileService.cs              # File handling service
│       ├── MergeService.cs             # Document merging orchestration
│       ├── PdfService.cs               # PDF operations
│       └── WordService.cs              # Word document operations
└── bin/Release/                        # Compiled executable location
```

## Building the Application

### Prerequisites
- .NET 8.0 SDK or later
- Windows OS (WPF is Windows-specific)
- Visual Studio 2022 or later (recommended) or Visual Studio Code with C# extension

### Build Instructions

#### Using .NET CLI:
```bash
dotnet restore PDFWordMerger/PDFWordMerger.csproj
dotnet build PDFWordMerger/PDFWordMerger.csproj --configuration Release
```

#### Using Visual Studio:
1. Open `PDFWordMerger.sln` in Visual Studio
2. Select "Release" configuration
3. Build → Build Solution (Ctrl+Shift+B)

### Running the Application
After building, the executable will be located at:
```
PDFWordMerger/bin/Release/net8.0-windows/PDFWordMerger.exe
```

Double-click the .exe file to launch the application.

## Usage Guide

### Adding Documents
1. **File Browser**: Click "Add Files" button to select documents
2. **Drag & Drop**: Drag PDF or Word files directly onto the window
3. **Multi-Select**: Hold Ctrl/Shift to select multiple files at once

### Organizing Documents
1. **Select** a document in the list
2. **Move Up/Down** using the arrow buttons
3. **Delete** unwanted files with the 🗑 Delete button
4. **Auto-Sort** by date or name using the sort buttons

### Merging Documents
1. **Preview**: Click "📄 Preview Merge" to see the merge plan
2. **Export**: Click "💾 Export to PDF" to save the merged document
3. **Choose Location**: Select output filename and directory
4. **Wait**: Progress bar shows merge operation status
5. **Complete**: Success message displays output location

### Understanding the Interface

#### Left Panel - File List
- Shows all selected documents in merge order
- Displays file type icon, name, path, page count, and modification date
- Current order number shown on the right

#### Right Panel - Preview
- Shows how documents will be merged
- Displays total page count for each document
- Auto-sort buttons for quick organization

#### Bottom Section
- **Summary**: Total documents and pages count
- **Action Buttons**: Preview and Export operations
- **Status Bar**: Current operation status and progress

## Key Features Explained

### Page Numbering
- All pages receive sequential numbers starting from 1
- Numbers appear centered at the bottom of each page
- Formatting maintained across all document types

### Word Document Handling
- DOCX files parsed using OpenXML SDK
- Content extracted and converted to PDF format
- Text formatting (bold, font size, alignment) preserved
- Tables and complex elements handled gracefully

### PDF Processing
- Native PDF files merged directly
- Page count detection from PDF metadata
- Page-by-page copying maintains quality and formatting

### Error Handling
- File access errors caught and reported
- Corrupt document handling with user feedback
- Invalid file type filtering
- Temporary file cleanup on error or success

## Known Limitations

1. **Legacy Word Files (.doc)**: Page count estimation based on file size
2. **Complex Formatting**: Some advanced Word features may not convert perfectly
3. **Windows Only**: WPF requires Windows operating system
4. **Large Files**: Very large documents (100+ MB) may take longer to process

## Troubleshooting

### Application Won't Start
- Ensure .NET 8.0 Runtime is installed
- Check Windows version compatibility (Windows 10/11 recommended)

### File Won't Load
- Verify file is not corrupted
- Check file permissions (read access required)
- Ensure file format is supported (.pdf, .doc, .docx)

### Merge Fails
- Check disk space for output location
- Verify write permissions for output directory
- Close output file if already open in another program

## License

This project is provided as-is for educational and professional use.

## Support

For issues, feature requests, or questions, please create an issue in the repository.

---

**Built with ❤️ using .NET 8 and WPF**
