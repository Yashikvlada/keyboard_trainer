using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace st.kbt
{
    interface VKey
    {
        bool IsThisKeyPressed(System.Windows.Input.Key key);
        bool IsItBaseKey(string key);
        bool IsItAltKey(string key);
        string ToString();
        VKey GetChangedKey();
        float GetSize();
    }
    /// <summary>
    /// Клавиша с двумя значениями (base, alt) в формате char
    /// </summary>
    class DoubleKey:VKey
    {
        char baseSymb_;
        char altSymb_;
        float size_;
        string keyName_;

        public char BaseSymb { get {return baseSymb_; } }
        public char AltSymb { get { return altSymb_; } }

        /// <param name="baseSymb">Основное значение</param>
        /// <param name="altSymb">Альтернативное значение</param>
        /// <param name="keyName">Название клавиши(из класса Key)</param>
        /// <param name="size">Относительный размер клавиши (напр. 1-обычная, 6-пробел)</param>
        public DoubleKey(char baseSymb, char altSymb, string keyName, float size)
        {
            baseSymb_ = baseSymb;
            altSymb_ = altSymb;
            size_ = size;
            keyName_ = keyName;
        }

        public override string ToString()
        {
            return baseSymb_.ToString();
        }

        public VKey GetChangedKey()
        {
            return new DoubleKey(altSymb_,baseSymb_,keyName_, size_);
        }

        public float GetSize()
        {
            return size_;
        }

        public bool IsThisKeyPressed(System.Windows.Input.Key key)
        {
            return keyName_.Equals(key.ToString());
        }

        public bool IsItBaseKey(string key)
        {
            return key.Equals(baseSymb_.ToString());
        }

        public bool IsItAltKey(string key)
        {
            return key.Equals(altSymb_.ToString());
        }
    }
    /// <summary>
    /// Клавиша с одним значением в формате string
    /// </summary>
    class SimpleKey:VKey
    {
        string baseSymb_;
        float size_;
        string keyName_;

        /// <param name="baseSymb">Отображаемое имя кнопки</param>
        /// <param name="keyName">Название клавиши(из класса Key)</param>
        /// <param name="size">Относительный размер клавиши (напр. 1-обычная, 6-пробел)</param>
        public SimpleKey(string baseSymb, string keyName, float size)
        {
            baseSymb_ = baseSymb;
            size_ = size;
            keyName_ = keyName;
        }

        public override string ToString()
        {
            return baseSymb_;
        }

        public VKey GetChangedKey()
        {
            return this;
        }

        public float GetSize()
        {
            return size_;
        }

        public bool IsItAltKey(string key)
        {
            return false;
        }

        public bool IsItBaseKey(string key)
        {
            return key.Equals(baseSymb_);
        }

        public bool IsThisKeyPressed(Key key)
        {
            return keyName_.Equals(key.ToString());
        }
    }

}
