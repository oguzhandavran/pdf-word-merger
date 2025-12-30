using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PDFWordMerger.Models;
using PDFWordMerger.Services;

namespace PDFWordMerger.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private readonly MergeService _mergeService;
        private DocumentFile? _selectedDocument;
        private string _statusMessage = "Ready";
        private bool _isProcessing;
        private double _progressValue;

        public ObservableCollection<DocumentFile> DocumentFiles { get; set; }

        public DocumentFile? SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                _selectedDocument = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public int TotalDocuments => DocumentFiles.Count;
        public int TotalPages => DocumentFiles.Sum(d => d.PageCount);

        public ICommand AddFilesCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand DeleteFileCommand { get; }
        public ICommand AutoSortCommand { get; }
        public ICommand AutoSortByNameCommand { get; }
        public ICommand PreviewMergeCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        public MainWindowViewModel()
        {
            DocumentFiles = new ObservableCollection<DocumentFile>();
            _fileService = new FileService();
            _mergeService = new MergeService();

            AddFilesCommand = new RelayCommand(_ => AddFiles());
            MoveUpCommand = new RelayCommand(_ => MoveUp(), _ => CanMoveUp());
            MoveDownCommand = new RelayCommand(_ => MoveDown(), _ => CanMoveDown());
            DeleteFileCommand = new RelayCommand(_ => DeleteFile(), _ => SelectedDocument != null);
            AutoSortCommand = new RelayCommand(_ => AutoSortByDate());
            AutoSortByNameCommand = new RelayCommand(_ => AutoSortByName());
            PreviewMergeCommand = new RelayCommand(_ => PreviewMerge(), _ => DocumentFiles.Count > 0);
            ExportToPdfCommand = new RelayCommand(async _ => await ExportToPdf(), _ => DocumentFiles.Count > 0);

            DocumentFiles.CollectionChanged += (s, e) =>
            {
                UpdateOrderNumbers();
                OnPropertyChanged(nameof(TotalDocuments));
                OnPropertyChanged(nameof(TotalPages));
            };
        }

        private void AddFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Supported Documents|*.pdf;*.doc;*.docx|PDF Files|*.pdf|Word Documents|*.doc;*.docx|All Files|*.*",
                Title = "Select PDF or Word Documents"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AddFilesToList(openFileDialog.FileNames);
            }
        }

        public void AddDroppedFiles(string[] files)
        {
            var supportedFiles = files.Where(f =>
            {
                string ext = Path.GetExtension(f).ToLower();
                return ext == ".pdf" || ext == ".doc" || ext == ".docx";
            }).ToArray();

            if (supportedFiles.Length > 0)
            {
                AddFilesToList(supportedFiles);
            }
            else
            {
                MessageBox.Show("No supported files found. Please drop PDF or Word documents.", "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AddFilesToList(string[] filePaths)
        {
            StatusMessage = "Loading files...";
            IsProcessing = true;
            ProgressValue = 0;

            try
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    var doc = new DocumentFile(filePaths[i]);
                    
                    int pageCount = await Task.Run(() => _fileService.GetPageCount(filePaths[i]));
                    doc.PageCount = pageCount;
                    
                    DocumentFiles.Add(doc);
                    
                    ProgressValue = ((i + 1) / (double)filePaths.Length) * 100;
                }

                StatusMessage = $"Added {filePaths.Length} file(s) successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error loading files";
            }
            finally
            {
                IsProcessing = false;
                ProgressValue = 0;
            }
        }

        private bool CanMoveUp()
        {
            return SelectedDocument != null && DocumentFiles.IndexOf(SelectedDocument) > 0;
        }

        private void MoveUp()
        {
            if (SelectedDocument == null) return;

            int index = DocumentFiles.IndexOf(SelectedDocument);
            if (index > 0)
            {
                DocumentFiles.Move(index, index - 1);
                UpdateOrderNumbers();
                StatusMessage = $"Moved '{SelectedDocument.FileName}' up";
            }
        }

        private bool CanMoveDown()
        {
            return SelectedDocument != null && DocumentFiles.IndexOf(SelectedDocument) < DocumentFiles.Count - 1;
        }

        private void MoveDown()
        {
            if (SelectedDocument == null) return;

            int index = DocumentFiles.IndexOf(SelectedDocument);
            if (index < DocumentFiles.Count - 1)
            {
                DocumentFiles.Move(index, index + 1);
                UpdateOrderNumbers();
                StatusMessage = $"Moved '{SelectedDocument.FileName}' down";
            }
        }

        private void DeleteFile()
        {
            if (SelectedDocument == null) return;

            string fileName = SelectedDocument.FileName;
            DocumentFiles.Remove(SelectedDocument);
            UpdateOrderNumbers();
            StatusMessage = $"Removed '{fileName}'";
        }

        private void AutoSortByDate()
        {
            var sorted = DocumentFiles.OrderBy(d => d.ModifiedDate).ToList();
            DocumentFiles.Clear();
            foreach (var doc in sorted)
            {
                DocumentFiles.Add(doc);
            }
            UpdateOrderNumbers();
            StatusMessage = "Files sorted by modification date";
        }

        private void AutoSortByName()
        {
            var sorted = DocumentFiles.OrderBy(d => d.FileName).ToList();
            DocumentFiles.Clear();
            foreach (var doc in sorted)
            {
                DocumentFiles.Add(doc);
            }
            UpdateOrderNumbers();
            StatusMessage = "Files sorted alphabetically by name";
        }

        private void PreviewMerge()
        {
            string preview = "Merge Preview:\n\n";
            int totalPages = 0;

            for (int i = 0; i < DocumentFiles.Count; i++)
            {
                var doc = DocumentFiles[i];
                preview += $"{i + 1}. {doc.FileName}\n";
                preview += $"   Type: {doc.FileType}\n";
                preview += $"   Pages: {doc.PageCount}\n";
                preview += $"   Page numbers: {totalPages + 1} - {totalPages + doc.PageCount}\n\n";
                totalPages += doc.PageCount;
            }

            preview += $"Total pages in merged document: {totalPages}\n";
            preview += "Page numbers will be added to all pages.";

            MessageBox.Show(preview, "Merge Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ExportToPdf()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Save Merged PDF",
                FileName = $"Merged_Document_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StatusMessage = "Merging documents...";
                IsProcessing = true;
                ProgressValue = 0;

                try
                {
                    var operation = new MergeOperation
                    {
                        Documents = DocumentFiles.ToList(),
                        OutputPath = saveFileDialog.FileName,
                        AddPageNumbers = true
                    };

                    var progress = new Progress<double>(value =>
                    {
                        ProgressValue = value;
                    });

                    bool success = await Task.Run(() => _mergeService.MergeDocuments(operation, progress));

                    if (success)
                    {
                        StatusMessage = $"Successfully created: {Path.GetFileName(saveFileDialog.FileName)}";
                        MessageBox.Show($"PDF successfully created!\n\nLocation: {saveFileDialog.FileName}\n\nTotal pages: {TotalPages}", 
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "Merge failed";
                        MessageBox.Show($"Error: {operation.ErrorMessage}", "Merge Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = "Error during merge";
                    MessageBox.Show($"Error merging documents: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsProcessing = false;
                    ProgressValue = 0;
                }
            }
        }

        private void UpdateOrderNumbers()
        {
            for (int i = 0; i < DocumentFiles.Count; i++)
            {
                DocumentFiles[i].Order = i + 1;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
