using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

/**
 * Классы отвечающие за работу противника
 * 
 * Состояния доски хранятся в byte[]
 * К примеру:
 * 1 2 3
 * 4 5 6
 * 7 8 9
 * будет выглядеть так:
 * [1, 2, 3, 4, 5, 6, 7, 8, 9]
 * 
 * В массиве состояния:
 * 0 - не занятая ячейка
 * 1 - ячейка занята игроком (вы)
 * 2 - ячейка занята противником
 */
namespace tic_tac_toe_endgame.Classes
{
    /**
	 * Класс отвечающий за логику поведения противника
	 */
    [Serializable]
    public class c__ai
    {
        private int            i__wins_count      { get; set; } // Количество побед
        private List<c__state> l__cs__states      { get; set; } // Состояния досок, которые "помнит" противник
        private List<int>      l__i__current_game { get; set; } // Инексы состояний досок, которые были в текущей игре


        /**
		 * Конструктор
		 */
        public c__ai()
        {
            i__wins_count = 0; // Установить количество побед

            l__cs__states      = new List<c__state>(); // Инициализировать "память" противника
            l__i__current_game = new List<int>();      // Инициализировать "память" текущей игры
        }


        /**
		 * Сделать ход
		 * 
		 * @param  byte[]  a__b__current_state  Текущее состояние доски 
		 * 
		 * @return  byte[]  Состояние доски после хода
		 */
        public byte[] make_move(byte[] a__b__current_state)
        {
            c__state cs__current_state = null; // Будет содержать текущий ход (пока null)

            //
            // Попытаться найти текущий ход в памяти
            //
            foreach (c__state cs__item in l__cs__states)
            {
                //
                // Если текущих ход есть в памяти, то использовать значения из памяти
                //
                if (cs__item.get_state() == a__b__current_state)
                {
                    cs__current_state = cs__item; // Установить текущий ход

                    break; // Выйти из цикла
                }
            }

            //
            // Если ход не удалось "вспомнить", то создать новый ход
            //
            if (cs__current_state == null)
            {
                cs__current_state = new c__state(a__b__current_state); // Создать новый ход

                l__cs__states.Add(cs__current_state); // Добавить ход в память
            }

            l__i__current_game.Add(l__cs__states.IndexOf(cs__current_state)); // Записать ход

            return cs__current_state.select_move(); // Вернуть состояние доски после хода противника
        }


        /**
		 * Действия при выигрыше
		 */
        public void win()
        {
            i__wins_count++; // Засчитать победу

            foreach (int item in l__i__current_game)
            {
                l__cs__states[item].cm__best_move.weight_up();
            }

            l__i__current_game.Clear(); // Удалить воспоминания о последней игре
        }


        /**
		 * Действия при проигрыше
		 */
        public void loose()
        {
            l__cs__states[l__i__current_game.LastOrDefault()].cm__best_move.weight_down();

            l__i__current_game.Clear(); // Удалить воспоминания о последней игре
        }


        /**
		 * Получить количество побед
		 * 
		 * @return  int  Количество побед
		 */
        public int get_wins_count()
        {
            return i__wins_count;
        }
    }



    /**
	 * Класс состояния доски
	 */
    [Serializable]
    public class c__state
    {
        private byte[]        a__b__state { get; set; } // Состояние доски
        private List<c__move> l__cm__move { get; set; } // Возможные ходы

        public c__move cm__best_move { get; set; } // Лучший ход

        Random random = new Random();  // Инициализировать генератор случайных чисел


        /** 
		 * Конструктор класса
		 * 
		 * @param  byte[]  a__b__state_to_set  Состояние доски
		 */
        public c__state(byte[] a__b__state_to_set)
        {
            a__b__state = a__b__state_to_set; // Установить состояние

            l__cm__move = new List<c__move>(); // Установить список возможных ходов

            set_all_moves(); // Сгенерировать все возможные ходы
        }


        /**
		 * Сгенерировать все возможные ходы
		 */
        private void set_all_moves()
        {
            for (int i__1 = 0; i__1 < 9; i__1++)
            {
                //
                // Если ячейка на доске еще не занята, то
                // сгенерировать новый возможный ход
                //
                if (a__b__state[i__1] == 0)
                {
                    byte[] a__b__new_move = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Создать комбинацию для нового хода

                    //
                    // Приравнять переменную новой комбинации и текущего хода
                    //
                    for (int i__2 = 0; i__2 < 9; i__2++)
                    {
                        a__b__new_move[i__2] = a__b__state[i__2];
                    }

                    a__b__new_move.SetValue((byte)2, i__1); // "Противник делает ход"											


                    c__move cm__new_move = new c__move(a__b__new_move, (byte)random.Next(1, 5)); // Создать новый ход

                    l__cm__move.Add(cm__new_move); // Добавить новый ход в список возможных ходов
                }
            }
        }


        /**
		 * Определить самый успешный ход и сделать его
		 * 
		 * @return  byte[]  Состояние доски после хода противника
		 */
        public byte[] select_move()
        {

            int i__sum_weight_in_state; // Сумма весов всех ходов в данном состоянии доски

            int i__move_selector;       // Селектор хода

            int i__sum_selector = 0;    // Нужен для выбора хода


            //
            // Будет содержать самый успешный ход,
            // пока что равен первому ходу из списка ходов
            //
            cm__best_move = l__cm__move[0];


            l__cm__move.OrderBy(o => o.get_weight()); // Отсортировать ходы во возростанию веса

            i__sum_weight_in_state = l__cm__move.Sum(o => o.get_weight()); // Подсчитать сумму весов ходов

            i__move_selector = random.Next(0, i__sum_weight_in_state); // Сгенерировать селектор


            //
            // Определить какой ход будет сделан с помощью селектора
            // 
            foreach (c__move cm__item in l__cm__move)
            {
                if (i__move_selector <= i__sum_selector)
                {
                    return cm__item.get_move(); // Вернуть состояние доски после хода противника                    
                }

                i__sum_selector += cm__item.get_weight(); // Добавить вес к селектору 
            }


            //
            // Если не удалось найти ход, то вернуть ход с максимальным весом
            //       
            return l__cm__move.Last().get_move();
        }


        /**
		 * Вернуть состояние доски
		 * 
		 * @return  byte[]  Состояние доски
		 */
        public byte[] get_state()
        {
            return a__b__state; // Вернуть состояние доски
        }
    }



    /**
	 * Класс хода
	 * 
	 * Содержит комбинацию хода и вес
	 */
    [Serializable]
    public class c__move
    {
        private byte[] a__b__move    { get; set; } // Комбинация хода
        private byte   b__weght      { get; set; } // Вес
        private byte   b__max_weight { get; set; } // Максимальный вес


        /**
		 * Конструктор класса
		 * 
		 * Устанавливается комбинация хода и его вес
		 * 
		 * @param  byte[]  a__b__move_to_set  Комбинация хода
		 * @param  byte    b__random          Случайный вес от 1 до 5
		 */
        public c__move(byte[] a__b__move_to_set, byte b__random)
        {
            a__b__move = a__b__move_to_set; // Установить комбинацию


            b__max_weight = 5; // Установить значение максимального веса

            b__weght = b__random; // Установить случайный вес от 1 до 5
        }


        /**
		 * Повысить вес
		 */
        public void weight_up()
        {
            //
            // Если вес не максимален то...
            //
            if (b__weght < b__max_weight)
            {
                b__weght++; // Повысить вес на 1
            }
        }


        /**
		 * Понизить вес
		 */
        public void weight_down()
        {
            //
            // Если вес не минимален то...
            //
            if (b__weght > 1)
            {
                b__weght--; // Понизить вес на 1
            }
        }


        /**
		 * Получить значение веса
		 * 
		 * @return  byte  Значение веса
		 */
        public int get_weight()
        {
            return b__weght; // Вернуть значение веса
        }


        /**
		 * Получить комбинацию хода
		 * 
		 * @return  byte[]  Комбинация хода
		 */
        public byte[] get_move()
        {
            return a__b__move; // Вернуть комбинацию хода
        }
    }
}
