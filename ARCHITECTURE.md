# PDF & Word Merger - Architecture Documentation

## Overview
This document describes the architecture, design patterns, and technical implementation of the PDF & Word Document Merger application.

## Technology Stack

### Core Frameworks
- **.NET 8.0**: Modern .NET framework with long-term support
- **WPF (Windows Presentation Foundation)**: Desktop UI framework
- **C# 12**: Latest C# language features

### Key Libraries
- **iText7 (8.0.5)**: Professional PDF manipulation
  - itext.kernel: Core PDF operations
  - itext.layout: Document layout and formatting
  - itext.bouncy-castle-adapter: Security features
- **DocumentFormat.OpenXml (3.1.0)**: Microsoft Office document processing

## Architecture Pattern: MVVM

The application follows the Model-View-ViewModel (MVVM) pattern for clean separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                         View Layer                       │
│  (MainWindow.xaml - User Interface)                     │
│  - Data Binding                                         │
│  - Visual Elements                                      │
│  - User Interactions                                    │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ Data Binding
                     │ Commands
                     ↓
┌─────────────────────────────────────────────────────────┐
│                      ViewModel Layer                     │
│  (MainWindowViewModel.cs)                               │
│  - Business Logic                                       │
│  - Command Handlers                                     │
│  - State Management                                     │
│  - UI Notifications (INotifyPropertyChanged)            │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ Service Calls
                     │ Data Access
                     ↓
┌─────────────────────────────────────────────────────────┐
│                      Model Layer                        │
│  (DocumentFile.cs, MergeOperation.cs)                  │
│  - Data Structures                                      │
│  - Business Entities                                    │
└─────────────────────────────────────────────────────────┘
                     │
                     │
                     ↓
┌─────────────────────────────────────────────────────────┐
│                     Service Layer                       │
│  (FileService, MergeService, PdfService, WordService)  │
│  - File Operations                                      │
│  - Document Processing                                  │
│  - Conversion Logic                                     │
└─────────────────────────────────────────────────────────┘
```

## Project Structure

```
PDFWordMerger/
├── Views/                          # UI Components (future expansion)
├── ViewModels/                     # View Models and Commands
│   ├── MainWindowViewModel.cs      # Main application logic
│   └── RelayCommand.cs             # Command implementation
├── Models/                         # Data models
│   ├── DocumentFile.cs             # Document metadata
│   └── MergeOperation.cs           # Merge operation data
├── Services/                       # Business logic services
│   ├── FileService.cs              # File handling
│   ├── MergeService.cs             # Document merging
│   ├── PdfService.cs               # PDF operations
│   └── WordService.cs              # Word document processing
├── Converters/                     # Value converters
│   └── BoolToVisibilityConverter.cs
├── MainWindow.xaml                 # Main window UI
├── MainWindow.xaml.cs              # Main window code-behind
├── App.xaml                        # Application resources
└── App.xaml.cs                     # Application entry point
```

## Core Components

### 1. MainWindowViewModel
**Purpose**: Orchestrates user interactions and application state

**Responsibilities**:
- File selection and drag-drop handling
- Document list management
- UI state updates (status messages, progress)
- Command coordination

**Key Properties**:
- `DocumentFiles`: Observable collection of documents
- `SelectedDocument`: Currently selected document
- `StatusMessage`: Status bar message
- `IsProcessing`: Processing state flag
- `TotalDocuments`, `TotalPages`: Summary statistics

**Commands**:
- `AddFilesCommand`: Open file dialog
- `MoveUpCommand`: Reorder document up
- `MoveDownCommand`: Reorder document down
- `DeleteFileCommand`: Remove document
- `AutoSortCommand`: Sort by date
- `AutoSortByNameCommand`: Sort by name
- `PreviewMergeCommand`: Show merge preview
- `ExportToPdfCommand`: Execute merge operation

### 2. DocumentFile Model
**Purpose**: Represents a single document in the merge list

**Properties**:
- `FilePath`: Full path to the file
- `FileName`: Display name
- `FileType`: Type description (PDF/Word)
- `FileTypeIcon`: Visual emoji icon
- `ModifiedDate`: Last modification timestamp
- `PageCount`: Number of pages
- `Order`: Position in merge sequence

**Features**:
- Implements `INotifyPropertyChanged` for UI updates
- Auto-detects file type from extension
- Reads file system metadata

### 3. MergeService
**Purpose**: Orchestrates the document merging process

**Workflow**:
1. **Conversion Phase**
   - Convert all documents to PDF format
   - Word documents → temporary PDFs
   - Native PDFs → copied to temp directory

2. **Merge Phase**
   - Create output PDF document
   - Copy pages from each source PDF
   - Maintain document order

3. **Numbering Phase**
   - Add sequential page numbers
   - Centered at bottom of each page
   - Format: "Page N"

4. **Cleanup Phase**
   - Delete temporary files
   - Remove temporary directory

**Error Handling**:
- Try-catch around each phase
- Temporary file cleanup on error
- User-friendly error messages

### 4. PdfService
**Purpose**: Handle PDF-specific operations

**Methods**:
- `GetPageCount(string pdfPath)`: Count pages in PDF
- `AddPageNumbersToPdf(PdfDocument, int startPage)`: Add page numbers
- `CopyPdfPages(...)`: Copy pages between PDFs

**Technologies**:
- iText7 PdfDocument API
- iText7 Layout API for text placement

### 5. WordService
**Purpose**: Handle Word document operations

**Methods**:
- `GetPageCount(string wordPath)`: Estimate page count
- `ConvertWordToPdf(string wordPath, string pdfPath)`: Convert to PDF

**Conversion Strategy**:
- Parse DOCX using OpenXML SDK
- Extract text content
- Apply basic formatting (bold, font size, alignment)
- Generate PDF using iText7

**Limitations**:
- Complex formatting may not convert perfectly
- Tables converted as text placeholders
- Images not currently supported
- .DOC files use size-based page estimation

### 6. FileService
**Purpose**: Coordinate file operations

**Methods**:
- `GetPageCount(string filePath)`: Route to appropriate service
- Determines file type by extension
- Delegates to PdfService or WordService

## Data Flow

### Adding Documents
```
User Action (Add Files/Drag-Drop)
    ↓
MainWindowViewModel.AddFilesToList()
    ↓
For each file:
    Create DocumentFile model
    ↓
    FileService.GetPageCount()
        ↓
        PdfService or WordService
    ↓
    Add to DocumentFiles collection
    ↓
UI automatically updates (data binding)
```

### Merging Documents
```
User clicks "Export to PDF"
    ↓
MainWindowViewModel.ExportToPdf()
    ↓
SaveFileDialog for output location
    ↓
Create MergeOperation model
    ↓
MergeService.MergeDocuments()
    ↓
    ConvertAllToPdf()
        ↓
        WordService.ConvertWordToPdf() for Word files
        ↓
        Copy PDF files
    ↓
    MergePdfsWithPageNumbers()
        ↓
        Copy all pages to output PDF
        ↓
        Add page numbers to all pages
    ↓
    Cleanup temporary files
    ↓
Success/Error message to user
```

## UI Binding Strategy

### XAML Data Binding
- Two-way binding for user input
- One-way binding for display data
- Command binding for buttons
- Collection binding for lists

### Example Binding:
```xaml
<!-- Observable Collection Binding -->
<ListBox ItemsSource="{Binding DocumentFiles}"/>

<!-- Property Binding -->
<TextBlock Text="{Binding StatusMessage}"/>

<!-- Command Binding -->
<Button Command="{Binding AddFilesCommand}"/>

<!-- Two-Way Binding -->
<ListBox SelectedItem="{Binding SelectedDocument}"/>
```

## Async Operations

### File Loading
- Async page count detection
- Non-blocking UI during file processing
- Progress reporting

### Document Merging
- Runs on background thread
- Progress updates via `IProgress<double>`
- UI updates marshaled to main thread

## Error Handling Strategy

### Levels of Error Handling

1. **Service Level**
   - Try-catch in each service method
   - Return default values on error
   - Log error context

2. **ViewModel Level**
   - Try-catch around async operations
   - User-friendly error messages
   - Status message updates

3. **UI Level**
   - MessageBox for critical errors
   - Status bar for informational messages
   - Visual feedback during processing

## Performance Considerations

### Optimization Strategies
1. **Lazy Loading**: Page counts loaded on-demand
2. **Async Operations**: Non-blocking file operations
3. **Streaming**: Large files processed in streams
4. **Temporary Files**: Intermediate files for memory efficiency
5. **Progress Reporting**: User feedback during long operations

### Memory Management
- `using` statements for disposable resources
- Temporary file cleanup
- Stream disposal after operations
- Large document handling via disk (not memory)

## Extensibility Points

### Future Enhancement Areas

1. **Additional File Formats**
   - Add services for other formats (PPT, ODT, etc.)
   - Implement converter interface

2. **Advanced Page Numbering**
   - Custom number formats
   - Different positions (header, footer, corner)
   - Page ranges and exclusions

3. **Document Editing**
   - Page rotation
   - Page deletion
   - Watermarks

4. **Batch Processing**
   - Save/load merge configurations
   - Preset templates
   - Command-line interface

5. **Cloud Integration**
   - Cloud storage providers
   - Online document services

## Testing Considerations

### Unit Testing Strategy
- Mock file system operations
- Test ViewModels in isolation
- Service layer unit tests
- Command execution tests

### Integration Testing
- End-to-end merge operations
- Various file type combinations
- Error scenario handling

### UI Testing
- WPF UI automation
- User interaction scenarios
- Visual regression testing

## Security Considerations

1. **File Access**
   - Read-only access to source files
   - Write access validation for output
   - Path traversal prevention

2. **Temporary Files**
   - Secure temporary directory creation
   - Cleanup on error
   - No sensitive data persistence

3. **PDF Security**
   - iText7 security features available
   - Potential for encryption support
   - Digital signature capability

## Build and Deployment

### Build Configuration
- **Debug**: Development with symbols
- **Release**: Optimized for production

### Output Structure
```
bin/Release/net8.0-windows/
├── PDFWordMerger.exe          # Main executable
├── PDFWordMerger.dll          # Application assembly
├── itext7.*.dll               # PDF libraries
├── DocumentFormat.OpenXml.dll # Word processing
└── [other dependencies]       # Framework dependencies
```

### Distribution
- Single folder deployment
- All dependencies included
- .NET 8 runtime required on target machine
- Windows 10/11 compatible

## Troubleshooting Guide

### Common Issues

**Build Errors**
- Enable Windows targeting on Linux build machines
- Restore NuGet packages before build
- Check .NET 8 SDK installation

**Runtime Errors**
- Verify file permissions
- Check disk space for temporary files
- Ensure .NET 8 runtime installed

**Conversion Issues**
- Complex Word formatting limitations
- Large file processing time
- Memory constraints with very large documents

---

**Version**: 1.0  
**Last Updated**: 2024  
**Framework**: .NET 8.0 / WPF
