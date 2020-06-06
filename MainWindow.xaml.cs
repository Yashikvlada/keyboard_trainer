using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;

namespace st.kbt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<Button> allButtons_ = new List<Button>();
        List<char> alphabet_ = new List<char>();
        SolidColorBrush defaultBtColor_ = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
        SolidColorBrush pressedBtColor_ = new SolidColorBrush(Color.FromRgb(0xB3, 0xD2, 0x8D));
        Timer tmr_ = new Timer();
        Random rnd_ = new Random();
        SolidColorBrush redBrush_ = new SolidColorBrush(Color.FromRgb(0xBC, 54, 0x4B));
        SolidColorBrush greenBrush_ = new SolidColorBrush(Color.FromRgb(76, 187, 23));
        int maxTimerMsec_ = 5000;

        public MainWindow()
        {
            InitializeComponent();

            CreateVirtKeyBoard();

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;
            
            this.SizeChanged += MainWindow_SizeChanged;
            
            SetupTimer();
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeKeyBoard();
        }
        //старт/стоп
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            tmr_.Enabled = !tmr_.Enabled;

            string buttonName;

            if (tmr_.Enabled)
                buttonName = "Stop";
            else
                buttonName = "Start";

            ((Button)sender).Content = buttonName;
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tmr_.Interval = maxTimerMsec_ / ((Slider)sender).Value;
            //букв в минуту
            Speed.Text = (60 / (tmr_.Interval / 1000)).ToString("F");

        }

        //-----------------создание клавиатуры
        private void CreateVirtKeyBoard()
        {
            //ряды кнопок
            AddDoubleKey_AnyOneToWrapPanel(FirstKBLine, '`', '~',Key.OemTilde.ToString());
            AddDoubleKeys_ArrNumbersToWrapPanel(FirstKBLine, "1234567890", "!@#$%^&*()");
            AddDoubleKey_AnyOneToWrapPanel(FirstKBLine, '-', '_', Key.OemMinus.ToString());
            AddDoubleKey_AnyOneToWrapPanel(FirstKBLine, '=', '+', Key.OemPlus.ToString());
            AddSimpleKeys_AnyOneToWrapPanel(FirstKBLine, "BackSpace", Key.Back.ToString(), 2);

            AddSimpleKeys_AnyOneToWrapPanel(SecondKBLine, "Tab", Key.Tab.ToString(), 1.5f);
            AddDoubleKeys_ArrLettersToWrapPanel(SecondKBLine, "qwertyuiop", "QWERTYUIOP");
            AddDoubleKey_AnyOneToWrapPanel(SecondKBLine, '[', '{', Key.OemOpenBrackets.ToString());
            AddDoubleKey_AnyOneToWrapPanel(SecondKBLine, ']', '}', Key.OemCloseBrackets.ToString());
            AddDoubleKey_AnyOneToWrapPanel(SecondKBLine, '\\', '|', Key.Oem5.ToString(), 1.5f);
            
            AddSimpleKeys_AnyOneToWrapPanel(ThirdKBLine, "CapsLock", Key.CapsLock.ToString(), 2);
            AddDoubleKeys_ArrLettersToWrapPanel(ThirdKBLine, "asdfghjkl", "ASDFGHJKL");
            AddDoubleKey_AnyOneToWrapPanel(ThirdKBLine, ';', ':', Key.OemSemicolon.ToString());
            AddDoubleKey_AnyOneToWrapPanel(ThirdKBLine, '\'', '"', Key.OemQuotes.ToString());
            AddSimpleKeys_AnyOneToWrapPanel(ThirdKBLine, "Enter", Key.Enter.ToString(), 2);

            AddSimpleKeys_AnyOneToWrapPanel(FourthKBLine, "LShift", Key.LeftShift.ToString(), 2.5f);
            AddDoubleKeys_ArrLettersToWrapPanel(FourthKBLine, "zxcvbnm", "ZXCVBNM");
            AddDoubleKey_AnyOneToWrapPanel(FourthKBLine, ',', '<', Key.OemComma.ToString());
            AddDoubleKey_AnyOneToWrapPanel(FourthKBLine, '.', '>', Key.OemPeriod.ToString());
            AddDoubleKey_AnyOneToWrapPanel(FourthKBLine, '/', '?', Key.OemQuestion.ToString());
            AddSimpleKeys_AnyOneToWrapPanel(FourthKBLine, "RShift", Key.RightShift.ToString(), 2.5f);

            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine, "LCtrl", Key.LeftCtrl.ToString(), 1.5f);
            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine, "WinKey", Key.LWin.ToString(), 1.5f);
            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine,"Alt", Key.System.ToString(), 1.5f);
            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine,"Space", Key.Space.ToString(), 6);
            //AddSimpleButtonsToWrapPanel(FifthKBLine,"RAlt", Key.RightAlt.ToString(), 1.5f);
            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine, "FN", Key.RWin.ToString(), 3f);
            var b = ComputePressedButton(Key.RWin);
            b.IsEnabled = false;
            AddSimpleKeys_AnyOneToWrapPanel(FifthKBLine, "RCtrl", Key.RightCtrl.ToString(), 1.5f);

            //создаем алфавит из клавиатуры
            CreateAlphabet();
        }       
        
        //вспомогательные методы (для удобного добавления клавиш)
        private void AddDoubleKeys_ArrNumbersToWrapPanel(WrapPanel wp, string symBase, string symbAlt, float size = 1)
        {
            for (int i = 0; i < symBase.Length; ++i)
            {
                AddDoubleKey_AnyOneToWrapPanel(wp, symBase[i], symbAlt[i], "D" + symBase[i].ToString(), size);
            }
        }
        private void AddDoubleKeys_ArrLettersToWrapPanel(WrapPanel wp,string symBase,string symbAlt, float size =1)
        {
            for (int i = 0; i < symBase.Length; ++i)
            {
                AddDoubleKey_AnyOneToWrapPanel(wp,symBase[i], symbAlt[i], symbAlt[i].ToString(), size);
            }
        }
        private void AddDoubleKey_AnyOneToWrapPanel(WrapPanel wp, char symBase, char symbAlt, string keyName, float size = 1)
        {
                DoubleKey newDK = new DoubleKey(symBase, symbAlt, keyName, size);

                Button newButton = new Button()
                {
                    Content = newDK,
                    Background=defaultBtColor_,
                    IsTabStop = false,
                    ClickMode = ClickMode.Hover
                };

                wp.Children.Add(newButton);

                allButtons_.Add(newButton);
            
        }
        private void AddSimpleKeys_AnyOneToWrapPanel(WrapPanel wp,string keySymbols,string keyName, float size =1)
        {
            Button newButton = new Button()
            {
                Content = new SimpleKey(keySymbols, keyName, size),
                Background=defaultBtColor_,
                IsTabStop = false,
                ClickMode = ClickMode.Hover
            };

            wp.Children.Add(newButton);

            allButtons_.Add(newButton);
        }

        private void ResizeKeyBoard()
        {
            foreach (var child in keyBoardGrid.Children)
            {
                if (child is WrapPanel)
                {
                    ResizeButtons(child as WrapPanel);
                }
            }
            keyBoardGrid.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void ResizeButtons(WrapPanel wp)
        {
            float length = 0;
            int buttons = 0;

            foreach (Button btn in wp.Children)
            {
                length += ((VKey)btn.Content).GetSize();
                ++buttons;
            }
            float fullWidth = (float)Width;
            float mult = (float)(fullWidth / length);

            foreach (Button btn in wp.Children)
            {
                btn.Width = ((VKey)btn.Content).GetSize() * (mult - btn.BorderThickness.Left - btn.BorderThickness.Right);
            }

        }
        private void CreateAlphabet()
        {
            foreach (var ch in allButtons_)
            {
                if (ch.Content is DoubleKey)
                {
                    alphabet_.Add(((DoubleKey)ch.Content).BaseSymb);
                    alphabet_.Add(((DoubleKey)ch.Content).AltSymb);
                }
            }
        }
        //-----------------создание клавиатуры

        //--------------нажатия клавиш
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {         
            if (e.Key==Key.CapsLock && !e.IsRepeat)
            {
                ChangeKeysCase();
            }
            else if((e.Key==Key.LeftShift|| e.Key == Key.RightShift) && !e.IsRepeat)
            {
                ChangeKeysCase();
            }
            try
            {
                var button = ComputePressedButton(e.Key);
                button.Background = pressedBtColor_;

                if (button.Content is DoubleKey)
                {
                    TextBlock ch = new TextBlock();
                    ch.Text= button.Content.ToString();
                    SPinputKey.Children.Add(ch);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }
        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {           
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ChangeKeysCase();
            }
            try
            {
                var button = ComputePressedButton(e.Key);
                button.Background = defaultBtColor_;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Button ComputePressedButton(Key key)
        {
            foreach(var b in allButtons_)
            {
                if (((VKey)b.Content).IsThisKeyPressed(key))
                    return b;
            }
            throw new Exception($"Incorrect key: {key.ToString()}");
        }
        private void ChangeKeysCase()
        {
            for (int i = 0; i < allButtons_.Count; ++i)
            {
                if (allButtons_[i].Content is DoubleKey)
                {
                    allButtons_[i].Content = ((DoubleKey)allButtons_[i].Content).GetChangedKey();
                }
            }
        }
        //--------------нажатия клавиш
        private List<Button> ComputeGeneratedKey(string key)
        {
            List<Button> buttons = new List<Button>();

            foreach (var b in allButtons_)
            {
                //нажата клавиша в текущем кейсе (вернем 1 кнопку, в которой она лежит)
                if (((VKey)b.Content).IsItBaseKey(key))
                {
                    buttons.Add(b);
                    return buttons;  
                }
                //нажата клавиша в альтернативном кейсе (вернем 2 кнопки, ее текущий кейс и шифт)
                else if (((VKey)b.Content).IsItAltKey(key))
                {
                    buttons.Add(b);
                    buttons.Add(ComputePressedButton(Key.LeftShift));
                    return buttons;
                }                  
            }
            throw new Exception($"Incorrect generated key: {key}");
        }

        //-------------таймер
        private void SetupTimer()
        {
            tmr_.Interval = maxTimerMsec_ / slider.Value;
            tmr_.Elapsed += new ElapsedEventHandler(Tmr__Elapsed);
            tmr_.AutoReset = true;
            tmr_.Enabled = false;
        }
        private void Tmr__Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(()=> 
            {
                //сравниваем ввод и рандом
                InputAndGeneratedKeysCompare();
                //чистим весь ввод
                SPinputKey.Children.Clear();

                //снимаем подсвеченные клавиши (красим в defaultBtColor_)
                int iLastGener = SPgeneratedKey.Children.Count - 1;
                if(iLastGener>=0)
                    ColorGeneratedKey((
                        SPgeneratedKey.Children[iLastGener] as TextBlock).Text, 
                        defaultBtColor_);

                //генерируем случайный символ (из алфавита)
                var generatedKey = GenerateKey();
                //добавляем
                SPgeneratedKey.Children.Add(new TextBlock() { Text = generatedKey });
                //подсвечиваем подсказку (красим генерируемую клавишу в красный)
                ColorGeneratedKey(generatedKey, redBrush_);
            });           
        }
        private void InputAndGeneratedKeysCompare()
        {
            int iLastGenerTxtBlk = SPgeneratedKey.Children.Count - 1;

            //сравниваем генерируемую и введенную
            if (iLastGenerTxtBlk >= 0)
            {
                int iLastInputTxtBlk = SPinputKey.Children.Count - 1;
  
                var lastGener = (TextBlock)SPgeneratedKey.Children[iLastGenerTxtBlk];

                bool isEqual = false;
                if (iLastInputTxtBlk >= 0)
                {
                    var lastInput = (TextBlock)SPinputKey.Children[iLastInputTxtBlk];
                    isEqual = lastGener.Text.Equals(lastInput.Text);
                }

                if (isEqual)
                {
                    lastGener.Background = greenBrush_;
                }
                else
                {
                    lastGener.Background = redBrush_;
                    Errors.Text = ((int.Parse(Errors.Text) + 1).ToString());
                }
            }
        }
        private bool isLastGeneratedAndInputEqual(int iLastGener, int iLastInput)
        {
            bool isEqual = false;
            if (iLastInput >= 0)
            {
                isEqual = 
                (SPgeneratedKey.Children[iLastGener] as TextBlock).Text ==
                (SPinputKey.Children[iLastInput] as TextBlock).Text;
            }

            return isEqual;
        }
        private string GenerateKey()
        {
            int index = rnd_.Next(alphabet_.Count);
            return alphabet_[index].ToString();
        }
        private void ColorGeneratedKey(string key, SolidColorBrush brush)
        {
            var buttons = ComputeGeneratedKey(key);
            foreach (var b in buttons)
                b.Background = brush;
        }
        //-------------таймер
    }
}
