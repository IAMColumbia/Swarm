﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using Windows.Storage.Pickers;
using Windows.Storage;
using System;
using System.Collections.Generic;
using XNASwarms;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.Serialization;


namespace XNASwarmsXAML.W8
{
    /// <summary>
    /// The root page used to display the game.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 _game;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            // Create the game.
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FileSavePicker exportPicker = new FileSavePicker();
            exportPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            exportPicker.FileTypeChoices.Add("XML", new List<string>() { ".xml" });
            exportPicker.DefaultFileExtension = ".xml";
            exportPicker.SuggestedFileName = "SwarmsSaves";
            StorageFile file = await exportPicker.PickSaveFileAsync();
            if (null != file)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (Stream outputStream = stream.AsStreamForWrite())
                    {
                        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                        xmlWriterSettings.Indent = true;
                        xmlWriterSettings.Encoding = new UTF8Encoding(false);
                        XmlSerializer serializer = new XmlSerializer(typeof(SaveAllSpecies));
                        var game = await SaveHelper.LoadGameFile("AllSaved");
                        using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, xmlWriterSettings))
                        {
                            serializer.Serialize(xmlWriter, game);
                        }
                        await outputStream.FlushAsync();
                    }
                    importstatus.Text = "Successfully exported @ " + DateTime.Now.ToString("t");
                }
            }
            else
            {
                importstatus.Text = "File was not exported @ " + DateTime.Now.ToString("t");
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FileOpenPicker importPicker = new FileOpenPicker();
            importPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            importPicker.FileTypeFilter.Add(".xml");
            importPicker.CommitButtonText = "Import";
            StorageFile file = await importPicker.PickSingleFileAsync();
            if (null != file)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (Stream inputStream = stream.AsStreamForRead())
                    {
                        SaveAllSpecies test;
                        XmlSerializer serializer = new XmlSerializer(typeof(SaveAllSpecies));
                        using (XmlReader xmlReader = XmlReader.Create(inputStream))
                        {
                            test = (SaveAllSpecies)serializer.Deserialize(xmlReader);
                            
                            importstatus.Text = "Successfully imported @ " + DateTime.Now.ToString("t");
                        }
                        await inputStream.FlushAsync();
                    }
                    importstatus.Text = "Successfully imported @ " + DateTime.Now.ToString("t");
                }
            }
            else
            {
                importstatus.Text = "File was not imported @ " + DateTime.Now.ToString("t");
            }
        }
    }
}
