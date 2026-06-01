using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace NOPRO
{
    public partial class AdminWindow : Window
    {
        private DatabaseManager _db = new DatabaseManager();
        private List<PromptItem> _prompts = new();

        public AdminWindow()
        {
            InitializeComponent();
            LoadPromptsFromDb();
        }

        private void LoadPromptsFromDb()
        {
            _prompts.Clear();

            // Промпты (Cp / Tg)
            var promptNames = new[]
{
    "CpMarker", "CpMainTitle", "CpAnchor",
    "CpOpinion", "CpMeta", "CpFAQ", "CpInCon", "CpSpell", "CpLegal", "CpNarr",
    "CpQuotes", "CpСhart", "CpBlockAd1", "CpBlockAd2",
    "TgInput", "TgConcl", "TgMeta", "TgTable", "TgFAQ", "TgAnchor", "TgOpinion",
    "TgAdd1", "TgAdd2", "CpMainPrompt"
};


            foreach (var name in promptNames)
            {
                var template = _db.GetPrompt(name) ?? "";
                _prompts.Add(new PromptItem { Name = name, Template = template });
            }

            // --- ДОБАВЛЯЕМ НОВЫЕ РЕСУРСЫ ---
            var trashValue = _db.GetResource("trash") ?? "";
            _prompts.Add(new PromptItem { Name = "RESOURCE_trash", Template = trashValue });

            var ngTrashValue = _db.GetResource("NGtrash") ?? "";
            _prompts.Add(new PromptItem { Name = "RESOURCE_NGtrash", Template = ngTrashValue });

            var bad_wordValue = _db.GetResource("bad_word") ?? "";
            _prompts.Add(new PromptItem { Name = "RESOURCE_bad_word", Template = bad_wordValue });
            // -----------------------------


            PromptsList.ItemsSource = _prompts;
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _prompts)
            {
                if (item.Name.StartsWith("RESOURCE_"))
                {
                    string key = item.Name.Substring("RESOURCE_".Length);
                    _db.SaveResource(key, item.Template);
                }
                else
                {
                    _db.SavePrompt(item.Name, item.Template);
                }
            }
            MessageBox.Show("Изменения сохранены в базу!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }




        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = "prompts_backup.json"
            };
            if (dialog.ShowDialog() == true)
            {
                var json = JsonSerializer.Serialize(_prompts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show($"Промпты сохранены в:\n{dialog.FileName}");
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var loaded = JsonSerializer.Deserialize<List<PromptItem>>(json);
                    if (loaded != null)
                    {
                        _prompts.Clear();
                        foreach (var p in loaded)
                        {
                            _prompts.Add(new PromptItem { Name = p.Name, Template = p.Template ?? "" });
                        }
                        PromptsList.ItemsSource = null;
                        PromptsList.ItemsSource = _prompts;
                        MessageBox.Show("Промпты загружены из файла. Нажмите «Применить», чтобы обновить базу.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }



    public class PromptItem
    {
        public string Name { get; set; } = "";
        public string? Template { get; set; }
    }



}