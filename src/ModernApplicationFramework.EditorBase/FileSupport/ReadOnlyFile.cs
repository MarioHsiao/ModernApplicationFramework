﻿using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public sealed class ReadOnlyFile : MafFile, IReadOnlyFile
    {
        //private static async Task DoSaveAs(IStorableDocument storableDocument)
        //{
        //    // Show user dialog to choose filename.
        //    var dialog = new SaveFileDialog {FileName = storableDocument.FileName};
        //    var filter = string.Empty;


        //    //TODO: This is for the Editor Base Only
        //    //var fileExtension = Path.GetExtension(storableDocument.FileName);
        //    //var fileType =
        //    //    IoC.GetAll<IEditorProvider>()
        //    //       .SelectMany(x => x.SupportedFileDefinitions)
        //    //       .SingleOrDefault(x => x.FileType.FileExtension == fileExtension);
        //    //if (fileType != null)
        //    //    filter = fileType.Name + "|*" + fileType.FileType.FileExtension + "|";

        //    filter += "All Files|*.*";
        //    dialog.Filter = filter;

        //    if (dialog.ShowDialog() != true)
        //        return;

        //    var filePath = dialog.FileName;

        //    // Save file.
        //    //await storableDocument.Save(filePath);
        //}

        //private bool CanSaveFile()
        //{
        //    return this is IStorableDocument;
        //}

        //private bool CanSaveFileAs()
        //{
        //    return this is IStorableDocument;
        //}


        //private async void SaveFile()
        //{
        //    if (!(this is IStorableDocument storableDocument))
        //        return;

        //    // If file has never been saved, show Save As dialog.
        //    if (storableDocument.IsNew)
        //    {
        //        await DoSaveAs(storableDocument);
        //        return;
        //    }
        //    // Save file.
        //    var filePath = storableDocument.FilePath;
        //    //await storableDocument.Save(filePath);
        //}

        //private async void SaveFileAs()
        //{
        //    if (!(this is IStorableDocument storableDocument))
        //        return;

        //    await DoSaveAs(storableDocument);
        //}


        public ReadOnlyFile(string path, string name) : base(path, name)
        {
        }

        public override Task Load(Action loadAction)
        {
            loadAction();
            return Task.CompletedTask;
        }

        public static ReadOnlyFile OpenExisting(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new ArgumentException("File was not found");
            var document = new ReadOnlyFile(filePath, System.IO.Path.GetFileName(filePath));
            return document;
        }
    }
}