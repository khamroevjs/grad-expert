using System;
using System.Collections.Generic;
using System.Text;

/*
 * Класс для преобразования числа в текст 
 * (c) Андрей Николаев 2013
 * 
 * Работа базируется и частично использует код
 * классов RSDN.RusNumber и RSDN.RusCurrency
 * http://rsdn.ru/article/files/dotnet/RusNumber.xml
 * 
 * Распространяется свободно
 */

namespace GradeExpertCRM
{
    /// <summary>
    /// Преобразует число в текст, обозначающий русские денежные единицы
    /// </summary>
    public class NumberInWordsRussianCurrency: INumberToWordsTranslate
    {
        //Минимально возможное значение транслируемого числа
        private const decimal MIN_NUMBER = 0;
        //Максимально возможное значение транслируемого числа
        private const decimal MAX_NUMBER = 999999999999.99m;

        #region NumbersInWord
        
        //Названия сотен
        private static string[] hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        //Названия десятков
        private static string[] tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        //Названия первых двух чисел в мужском роде
        private static string[] from0to2male = 
        {
                "", "один ", "два "
        };

        //Названия первых двух чисел в женском роде
        private static string[] from0to2female = 
        {
                "", "одна ", "две "
        };

        //Названия чисел в диапазоне от 3 до 19
        private static string[] from3to19 = 
        {
                "", "", "", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
        };
        
        //Название ноля
        private static string Zero = "ноль ";
        #endregion NumbersInWord

        /// <summary>
        /// Определяет пол числа
        /// </summary>
        private enum NumberSex
        {
            Male,
            Female
        }

        /// <summary>
        /// Вспомогательный класс для хранения информации о триаде чисел
        /// </summary>
        private class RussianCurrencyDesc
        {
            public NumberSex NumberSex { get; set; }
            public string NumberOne { get; set; }
            public string NumberTwo { get; set; }
            public string NumberFive { get; set; }
        }

        //Названия разрядов русской валюты
        private static List<RussianCurrencyDesc> rusNumbDesc = new List<RussianCurrencyDesc>
        { 
            new RussianCurrencyDesc {NumberSex = NumberSex.Female, 
                NumberOne = "копейка", NumberTwo = "копейки", NumberFive = "копеек"},
            new RussianCurrencyDesc {NumberSex = NumberSex.Male, 
                NumberOne = "рубль", NumberTwo = "рубля", NumberFive = "рублей"},
            new RussianCurrencyDesc {NumberSex = NumberSex.Female, 
                NumberOne = "тысяча", NumberTwo = "тысячи", NumberFive = "тысяч"},
            new RussianCurrencyDesc {NumberSex = NumberSex.Male, 
                NumberOne = "миллион", NumberTwo = "миллиона", NumberFive = "миллионов"},
            new RussianCurrencyDesc {NumberSex = NumberSex.Male, 
                NumberOne = "миллиард", NumberTwo = "миллиарда", NumberFive = "миллиардов"},
        };

        //Целая часть числа
        private long integralPart;
        //Дробная часть числа
        private int fractionalPart;
        //Число для трансляции
        private decimal numberToTranslate;
        //Отображать дробную часть или нет
        private bool isFractionalPartEnabled;

        /// <summary>
        /// Инициализирует новый класс, транслирующий число в текст
        /// </summary>
        /// <param name="numberToTranslate">Число для трансляции</param>
        /// <param name="isFractionalPartEnabled">Отображать дробную часть или нет</param>
        public NumberInWordsRussianCurrency(decimal numberToTranslate, bool isFractionalPartEnabled)
        {
            if (numberToTranslate < MIN_NUMBER || numberToTranslate > MAX_NUMBER)
                throw new ArgumentOutOfRangeException(
                    String.Format("Допустимый диапазон чисел - от {0} до {1}", MIN_NUMBER, MAX_NUMBER));

            this.numberToTranslate = numberToTranslate;
            this.isFractionalPartEnabled = isFractionalPartEnabled;
            integralPart = (long)Math.Truncate(numberToTranslate);
            fractionalPart = (int)(numberToTranslate % 1 * 100);
        }

        #region Props

        /// <summary>
        /// Целая часть числа
        /// </summary>
        public long IntegralPart
        { 
            get { return integralPart; } 
        }
        
        /// <summary>
        /// Дробная часть числа
        /// </summary>
        public int FractionalPart
        {
            get { return fractionalPart; }
        }

        /// <summary>
        /// Представляет наименьшее возможное значение для трансляции. Это поле является константой.
        /// </summary>
        public decimal MinValue
        {
            get { return MIN_NUMBER; }
        }

        /// <summary>
        /// Представляет наибольшее возможное значение для трансляции. Это поле является константой.
        /// </summary>
        public decimal MaxValue
        {
            get { return MAX_NUMBER; }
        }

        #endregion Props

        /// <summary>
        /// Получить текстовое представление числа
        /// </summary>
        /// <returns>Строка представляющая число</returns>
        public string Translate()
        {
            long integralPartConvert = integralPart;

            StringBuilder res = new StringBuilder();

            //Исключение для обработки нуля
            if (integralPartConvert == 0)
            {
                res.Append(Zero);
                res.Append(rusNumbDesc[1].NumberFive);
            }

            //Счетчик триад
            int i = 1;

            while (integralPartConvert > 0)
            {
                //Получаем текущую триаду
                int currentTriad = (int)(integralPartConvert % 1000);

                res.Insert(0, " ");
                //Добавляем размерность
                res.Insert(0, Case(currentTriad, i));
                //Добавляем текстовое представление триады
                res.Insert(0, ConvertTriad(currentTriad, rusNumbDesc[i].NumberSex));
                
                //Переходим к следующей триаде
                i++;
                integralPartConvert /= 1000;
            }

            //Убрать пробел в конце текста, если он есть
            if (res[res.Length - 1] == ' ')
                res.Remove(res.Length - 1, 1);
            
            //Добавить копейки
            if (isFractionalPartEnabled)
            {
                res.Append(", ");
                res.Append(fractionalPart.ToString());
                res.Append(" ");
                res.Append(Case(fractionalPart, 0));
            }

            //Преобразовать первую букву предложения в заглавную
            res[0] = char.ToUpper(res[0]);
            return res.ToString();
        }

        /// <summary>
        /// Преобразует триаду чисел в текст
        /// </summary>
        /// <param name="num">Триада чисел</param>
        /// <param name="sex">Пол числа</param>
        /// <returns>Текстовое представление числа</returns>
        private string ConvertTriad(int num, NumberSex sex)
        {
            if (num == 0) 
                return "";

            StringBuilder res = new StringBuilder(hunds[num / 100]);

            if (num % 100 < 20 && num % 100 > 2)
            {
                res.Append(from3to19[num % 100]);
            }
            else
            {
                res.Append(tens[num % 100 / 10]);
                res.Append(from3to19[num % 10]);

                if ((num % 10) < 3)
                {
                    if (sex == NumberSex.Male)
                        res.Append(from0to2male[num % 10]);
                    else
                        res.Append(from0to2female[num % 10]);
                }
            }

            return res.ToString();
        }

        /// <summary>
        /// Возвращает текстовое обозначение текущей триады (копейки->рубли->тысячи->... и т.д.)
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="lap">Номер триады</param>
        /// <returns></returns>
        private string Case(int val, int lap)
        {
            int t = (val % 100 > 20) ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return rusNumbDesc[lap].NumberOne;
                case 2:
                case 3:
                case 4: return rusNumbDesc[lap].NumberTwo;
                default: return rusNumbDesc[lap].NumberFive;
            }
        }

    }
}
