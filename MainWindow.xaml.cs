using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace NOPRO
{
    public partial class MainWindow : Window
    {
        DatabaseManager db = new DatabaseManager();

        public MainWindow()
        {

            InitializeComponent();
            // === ЗАГРУЗКА ТЕКСТА ПРИ СТАРТЕ ===
            // Замени имена TextBox (например, txtTitle, txtKeywords) на те, что у тебя в XAML

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InMarker))
                InMarker.Text = Properties.Settings.Default.InMarker;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InWidth))
                InWidth.Text = Properties.Settings.Default.InWidth;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InMainTitle))
                InMainTitle.Text = Properties.Settings.Default.InMainTitle;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InWidthData))
                InWidthData.Text = Properties.Settings.Default.InWidthData;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InNGrams))
                InNGrams.Text = Properties.Settings.Default.InNGrams;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InAnchorT))
                InAnchorT.Text = Properties.Settings.Default.InAnchorT;
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InAnchorL))
                InAnchorL.Text = Properties.Settings.Default.InAnchorL;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InOpinion))
                InOpinion.Text = Properties.Settings.Default.InOpinion;


            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthT_1))
                InDepthT_1.Text = Properties.Settings.Default.InDepthT_1;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthT_2))
                InDepthT_2.Text = Properties.Settings.Default.InDepthT_2;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthT_3))
                InDepthT_3.Text = Properties.Settings.Default.InDepthT_3;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthT_4))
                InDepthT_4.Text = Properties.Settings.Default.InDepthT_4;
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthT_5))
                InDepthT_5.Text = Properties.Settings.Default.InDepthT_5;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthV_1))
                InDepthV_1.Text = Properties.Settings.Default.InDepthV_1;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthV_2))
                InDepthV_2.Text = Properties.Settings.Default.InDepthV_2;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthV_3))
                InDepthV_3.Text = Properties.Settings.Default.InDepthV_3;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthV_4))
                InDepthV_4.Text = Properties.Settings.Default.InDepthV_4;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.InDepthV_5))
                InDepthV_5.Text = Properties.Settings.Default.InDepthV_5;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // === СОХРАНЕНИЕ ТЕКСТА ПРИ ЗАКРЫТИИ ===
            // Записываем текущий текст из полей в параметры
            Properties.Settings.Default.InMarker = InMarker.Text;
            Properties.Settings.Default.InMainTitle = InMainTitle.Text;
            Properties.Settings.Default.InWidth = InWidth.Text;
            Properties.Settings.Default.InWidthData = InWidthData.Text;
            Properties.Settings.Default.InNGrams = InNGrams.Text;
            Properties.Settings.Default.InAnchorT = InAnchorT.Text;
            Properties.Settings.Default.InAnchorL = InAnchorL.Text;
            Properties.Settings.Default.InOpinion = InOpinion.Text;

            Properties.Settings.Default.InDepthT_1 = InDepthT_1.Text;
            Properties.Settings.Default.InDepthT_2 = InDepthT_2.Text;
            Properties.Settings.Default.InDepthT_3 = InDepthT_3.Text;
            Properties.Settings.Default.InDepthT_4 = InDepthT_4.Text;
            Properties.Settings.Default.InDepthT_5 = InDepthT_5.Text;
            Properties.Settings.Default.InDepthV_1 = InDepthV_1.Text;
            Properties.Settings.Default.InDepthV_2 = InDepthV_2.Text;
            Properties.Settings.Default.InDepthV_3 = InDepthV_3.Text;
            Properties.Settings.Default.InDepthV_4 = InDepthV_4.Text;
            Properties.Settings.Default.InDepthV_5 = InDepthV_5.Text;

            // Обязательно сохраняем настройки на жесткий диск
            Properties.Settings.Default.Save();
        }




        // --- ЭТАП 1: ПАРСИНГ СТРУКТУРЫ (InWidth) ---
        private void InWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = InWidth.Text;
            if (string.IsNullOrWhiteSpace(input)) return;

            SectionsContainer.Children.Clear();

            var h1Match = Regex.Match(input, @"(?i)<h1>(.*?)</h1>");
            if (h1Match.Success) InMainTitle.Text = h1Match.Groups[1].Value.Trim();

            var h2Blocks = Regex.Split(input, @"(?i)(?=<h2>)");
            int h2Count = 0;

            foreach (var block in h2Blocks)
            {
                if (string.IsNullOrWhiteSpace(block) || !block.Contains("<h2>")) continue;
                h2Count++;

                var h2Text = Regex.Match(block, @"(?i)<h2>(.*?)</h2>").Groups[1].Value.Trim();
                CreateHeaderInput(h2Text, $"InH2_{h2Count}", 0);

                var h3Matches = Regex.Matches(block, @"(?i)<h3>(.*?)</h3>");
                for (int j = 0; j < h3Matches.Count; j++)
                {
                    CreateHeaderInput(h3Matches[j].Groups[1].Value.Trim(), $"InH3_{h2Count}_{j + 1}", 20);
                }
            }
        }

        private void CreateHeaderInput(string text, string name, double leftMargin)
        {
            Border card = new Border
            {
                Style = (Style)FindResource("RoundedPanel"),
                Margin = new Thickness(leftMargin, 5, 0, 5)
            };

            TextBox tb = new TextBox
            {
                Name = name,
                Text = text,
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.Transparent,
                Foreground = System.Windows.Media.Brushes.White
            };

            card.Child = tb;

            if (this.FindName(name) != null) this.UnregisterName(name);
            this.RegisterName(name, tb);
            SectionsContainer.Children.Add(card);
        }

        // --- ЭТАП 2: УМНАЯ ВСТАВКА И ЧИСТИЛЬЩИКИ ---
        private void InDepthT_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = InDepthT_1.Text;
            if (input.Contains("\n") || input.Contains("\r"))
            {
                string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    InDepthT_1.Text = lines[0].Trim();
                    if (lines.Length > 1) InDepthT_2.Text = lines[1].Trim();
                    if (lines.Length > 2) InDepthT_3.Text = lines[2].Trim();
                    if (lines.Length > 3) InDepthT_4.Text = lines[3].Trim();
                    if (lines.Length > 4) InDepthT_5.Text = lines[4].Trim();
                }
            }
        }

        /* Логика чистки */
        private void InWidthData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb || tb.Text == null) return;

            // Получаем текущий текст
            string input = tb.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            // Получаем список "мусорных" слов
            string trashRaw = db.GetResource("trash") ?? "";
            if (string.IsNullOrWhiteSpace(trashRaw)) return;

            // Разбиваем на слова, убираем пробелы и делаем нижний регистр
            var trashWords = new HashSet<string>(
                trashRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLowerInvariant()),
                StringComparer.OrdinalIgnoreCase
            );

            if (trashWords.Count == 0) return;

            // Разбиваем входной текст на слова (через запятую)
            var words = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .Where(s => !string.IsNullOrEmpty(s))
                             .ToList();

            // Фильтруем
            var filteredWords = words.Where(word => !trashWords.Contains(word)).ToList();

            // Собираем обратно
            string result = string.Join(", ", filteredWords);

            // Избегаем бесконечного цикла TextChanged
            if (result != input)
            {
                tb.TextChanged -= InWidthData_TextChanged;
                tb.Text = result;
                tb.CaretIndex = tb.Text.Length; // курсор в конец
                tb.TextChanged += InWidthData_TextChanged;
            }
        }

        /* Логика чистки */
        // --- ЭТАП 2: УМНАЯ ВСТАВКА И ЧИСТИЛЬЩИКИ ---
        private void InNGrams_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb || string.IsNullOrEmpty(tb.Text)) return;

            string input = tb.Text;

            // Проверяем, есть ли переносы строк (признак вставки столбика)
            if (input.Contains("\n") || input.Contains("\r"))
            {
                // Разделяем текст на отдельные строчки
                var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => line.Trim())
                                 .Where(line => !string.IsNullOrEmpty(line))
                                 .ToList();

                if (lines.Count == 0) return;

                // Загружаем список стоп-слов из БД
                string ngTrashRaw = db.GetResource("NGtrash") ?? "";
                var ngTrashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(ngTrashRaw))
                {
                    foreach (var item in ngTrashRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        ngTrashSet.Add(item.Trim());
                    }
                }

                // Фильтруем строки, исключая мусорные слова
                var filteredLines = lines.Where(line => !ngTrashSet.Contains(line)).ToList();

                // Собираем результат в одну строку через запятую
                string result = string.Join(", ", filteredLines);

                // Временно отключаем событие, чтобы избежать зацикливания при замене
                tb.TextChanged -= InNGrams_TextChanged;

                tb.Text = result;
                tb.CaretIndex = tb.Text.Length; // Переносим курсор в конец строки

                // Возвращаем событие на место
                tb.TextChanged += InNGrams_TextChanged;
            }
        }


        // --- ЭТАП 3: ГЛАВНЫЙ СБОРЩИК "СТАТЬЯ" ---
        ////private void CpMainPrompt_Click(object sender, RoutedEventArgs e)
        ////{
        ////    MessageBox.Show("Промпт скопирован Статья!", "It's a great result!");
        //}

        // --- ОБРАБОТКА ОВАЛОВ (Cp) ---

        private void CopyToClipboard(string promptName, string successMessage)
        {
            // 1. Берем сырой шаблон промпта из базы данных по его имени
            string template = db.GetPrompt(promptName) ?? "";

            // 2. Прогоняем через умный парсер, который заменит все {In...}, {Tg...}, {Cp...}
            string finalizedText = ProcessTemplateTags(template);

            // 3. Копируем готовый текст в буфер обмена Windows
            Clipboard.SetText(finalizedText);

            // 4. Показываем красивое уведомление
            MessageBox.Show(successMessage, "It's a great result!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CpMainPrompt_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpMainPrompt", "Промпт скопирован Статья!");
        }

        private void CpMarker_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpMarker", "Промпт скопирован Marker!");
        }

        private void CpMainTitle_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpMainTitle", "Промпт скопирован MainTitle!");
        }

        private void CpMeta_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpMeta", "Промпт скопирован Meta!");
        }

        private void CpAnchor_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpAnchor", "Промпт скопирован Anchor!");
        }

        private void CpCompany_Click(object sender, RoutedEventArgs e)
        {
            // Обрати внимание: в базе данных имя промпта "CpOpinion", а на кнопке написано CpCompany
            CopyToClipboard("CpOpinion", "Промпт скопирован Company & Avtor!");
        }

        private void CpFAQ_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpFAQ", "Промпт скопирован FAQ!");
        }

        private void CpInCon_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpInCon", "Промпт скопирован Ведение & Заключение!");
        }

        private void CpQuotes_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpQuotes", "Промпт скопирован цитаты!");
        }

        private void CpChart_Click(object sender, RoutedEventArgs e)
        {
            // В AdminWindow.xaml.cs у тебя зарегистрировано имя "CpСhart" (с русской 'С' или английской, проверим оба варианта)
            CopyToClipboard("CpСhart", "Промпт скопирован график!");
        }

        private void CpBlockAd1_Click(object sender, RoutedEventArgs e)
        {
            // В AdminWindow.xaml.cs этот промпт сохранен как "CpBlockAd1"
            CopyToClipboard("CpBlockAd1", "Промпт скопирован Доп блок 1!");
        }

        private void CpBlockAd2_Click(object sender, RoutedEventArgs e)
        {
            // В AdminWindow.xaml.cs этот промпт сохранен как "CpBlockAd2"
            CopyToClipboard("CpBlockAd2", "Промпт скопирован Доп блок 2!");
        }

        private void CpSpell_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpSpell", "Промпт скопирован Проверь орфографию!");
        }

        private void CpLegal_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpLegal", "Промпт скопирован Проверь закон!");
        }

        private void CpNarr_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard("CpNarr", "Промпт скопирован Проверь повествование!");
        }



        // --- ОБРАБОТКА КРУГОВ (Tg - ToggleButtons) ---
        // ToggleButtons не копируют, они просто включаются/выключаются
        // Их состояние проверяется в CpMainPrompt_Click

        // Если нужно, чтобы при клике на Tg копировался какой-то текст:
        private void TgInput_Click(object sender, RoutedEventArgs e)
        {
            // Можно добавить логику, если нужно
            // Например, показать подсказку
        }

        private void TgConcl_Click(object sender, RoutedEventArgs e)
        {
            // Аналогично
        }

        private void TgMeta_Click(object sender, RoutedEventArgs e) { }
        private void TgTable_Click(object sender, RoutedEventArgs e) { }
        private void TgFAQ_Click(object sender, RoutedEventArgs e) { }
        private void TgAnchor_Click(object sender, RoutedEventArgs e) { }
        private void TgOpinion_Click(object sender, RoutedEventArgs e) { }
        private void TgAdd1_Click(object sender, RoutedEventArgs e) { }
        private void TgAdd2_Click(object sender, RoutedEventArgs e) { }

        // --- ОБРАБОТКА РОМБОВ (Cl) ---
        private void ClMarker_Click(object sender, RoutedEventArgs e) => InMarker.Clear();

        private void ClMainTitle_Click(object sender, RoutedEventArgs e) => InMainTitle.Clear();

        private void ClDepth_Click(object sender, RoutedEventArgs e)
        {
            InDepthT_1.Clear(); InDepthV_1.Clear();
            InDepthT_2.Clear(); InDepthV_2.Clear();
            InDepthT_3.Clear(); InDepthV_3.Clear();
            InDepthT_4.Clear(); InDepthV_4.Clear();
            InDepthT_5.Clear(); InDepthV_5.Clear();
        }

        private void ClWidth_Click(object sender, RoutedEventArgs e)
        {
            InWidthData.Clear();
            InWidth.Clear();
        }

        private void ClNGrams_Click(object sender, RoutedEventArgs e) => InNGrams.Clear();

        private void ClCond_Click(object sender, RoutedEventArgs e)
        {
            InAnchorT.Clear();
            InAnchorL.Clear();
            InOpinion.Clear();
            TgInput.IsChecked = false;
            TgConcl.IsChecked = false;
            TgMeta.IsChecked = false;
            TgTable.IsChecked = false;
            TgFAQ.IsChecked = false;
            TgAnchor.IsChecked = false;
            TgOpinion.IsChecked = false;
            TgAdd1.IsChecked = false;
            TgAdd2.IsChecked = false;
        }

        private string GetFormattedSections()
        {
            string formatted = "";

            foreach (var child in SectionsContainer.Children)
            {
                if (child is Border border && border.Child is TextBox tb)
                {
                    if (tb.Name.StartsWith("InH2"))
                    {
                        formatted += $"<h2>{tb.Text}</h2>\n";
                    }
                    else if (tb.Name.StartsWith("InH3"))
                    {
                        formatted += $"<h3>{tb.Text}</h3>\n";
                    }
                }
            }
            return formatted;
        }

        private void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.Owner = this;
            adminWindow.ShowDialog();
        }


        private string ProcessTemplateTags(string template)
        {
            if (string.IsNullOrEmpty(template)) return "";

            string result = template;
            int maxIterations = 7; // Увеличили глубину для вложенных промптов
            int iteration = 0;

            // Регулярное выражение с \s* учитывает случайные пробелы: { InMarker } или {InMarker}
            var regex = new System.Text.RegularExpressions.Regex(@"\{\s*([A-Za-z0-9_]+)\s*\}");

            while (regex.IsMatch(result) && iteration < maxIterations)
            {
                iteration++;
                var matches = regex.Matches(result);
                bool anyReplaced = false;

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string fullTag = match.Value;          // Например, "{InMarker}" или "{ TgInput }"
                    string tagName = match.Groups[1].Value; // Чистое имя тега, например "InMarker" или "TgInput"

                    string replacedValue = "";
                    bool isResolved = false;

                    // 1. Проверяем спец-ресурс плохих слов bad_word
                    if (tagName == "bad_word")
                    {
                        string badWordsRaw = db.GetResource("bad_word") ?? "";
                        replacedValue = string.Join(", ", badWordsRaw
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w.Trim()));
                        isResolved = true;
                    }

                    // === ДОБАВЛЯЕМ СЮДА ПРОВЕРКУ ДЛЯ ИСПРАВЛЕННОЙ СТРУКТУРЫ ===
                    if (tagName == "InSect" || tagName == "InWidth")
                    {
                        // Вызываем твой готовый метод, который соберет измененные h2 и h3
                        replacedValue = GetFormattedSections();
                        isResolved = true;
                    }

                    // 2. Ищем элемент управления на форме MainWindow по его имени
                    FrameworkElement element = this.FindName(tagName) as FrameworkElement;

                    // Если это динамическое поле H2/H3, и FindName его не выдал напрямую, ищем в контейнере
                    if (element == null && (tagName.StartsWith("InH2_") || tagName.StartsWith("InH3_")))
                    {
                        foreach (var child in SectionsContainer.Children)
                        {
                            if (child is Border border && border.Child is TextBox tb && tb.Name == tagName)
                            {
                                element = tb;
                                break;
                            }
                        }
                    }

                    if (element != null)
                    {
                        if (element is TextBox textBox)
                        {
                            // Для обычных текстовых окон (In) берем текст
                            replacedValue = textBox.Text ?? "";
                            isResolved = true;
                        }
                        else if (element is System.Windows.Controls.Primitives.ToggleButton toggleButton)
                        {
                            // Для Кругов (Tg). Если кнопка нажата — берем её промпт из БД, иначе — пустота
                            if (toggleButton.IsChecked == true)
                            {
                                replacedValue = db.GetPrompt(tagName) ?? "";
                            }
                            else
                            {
                                replacedValue = "";
                            }
                            isResolved = true;
                        }
                    }



                    // 3. Если это не элемент формы (например, теги типа CpInput, CpMarker или промпты без кнопок), 
                    //    пытаемся найти готовый шаблон в базе данных
                    if (!isResolved)
                    {
                        string dbPrompt = db.GetPrompt(tagName);
                        if (dbPrompt != null)
                        {
                            replacedValue = dbPrompt;
                            isResolved = true;
                        }
                    }

                    // Если нашли хоть какое-то значение (или заменяем на пустоту, если поле не заполнено)
                    if (isResolved)
                    {
                        // Если внутри подставленного текста есть символ "{}", заменяем его на маркер (для обратной совместимости)
                        if (replacedValue.Contains("{}") && this.FindName("InMarker") is TextBox mBox)
                        {
                            replacedValue = replacedValue.Replace("{}", mBox.Text ?? "");
                        }

                        result = result.Replace(fullTag, replacedValue);
                        anyReplaced = true;
                    }
                }

                // Если за весь проход ни один тег не изменился, прерываем цикл во избежание зависания
                if (!anyReplaced) break;
            }

            return result;
        }





    }




}

