using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tic_tac_toe_endgame.Classes;

namespace tic_tac_toe_endgame
{
    public partial class Form1 : Form
    {
        public Random       rnd;              // Генератор случайных чисел
        public byte[]       a__b__field;      // Доска для игры
        public c__ai        ai__ai;           // Противник
        public List<Button> l__btn__ui_field; // Список кнопок, являющихся ячейками доски



        public Form1()
        {
            InitializeComponent();



            l__btn__ui_field = new List<Button>(); // Установить список для кнопок

            //
            // Заполняем список кнопками
            //
            l__btn__ui_field.Add(btn_0);
            l__btn__ui_field.Add(btn_1);
            l__btn__ui_field.Add(btn_2);
            l__btn__ui_field.Add(btn_3);
            l__btn__ui_field.Add(btn_4);
            l__btn__ui_field.Add(btn_5);
            l__btn__ui_field.Add(btn_6);
            l__btn__ui_field.Add(btn_7);
            l__btn__ui_field.Add(btn_8);


            rnd = new Random(); // Установить генератор случайных чисел

            a__b__field = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Установить доску для игры


            //
            // Проверить наличие файла из которого можно загрузить противника
            //
            if (File.Exists("ai.dat"))
            {
                //
                // Десериализовать противника из файла
                //
                BinaryFormatter bf__formatter = new BinaryFormatter(); // Инструмент для сериализации

                //
                // Открыть поток для чтения файла
                //
                using (FileStream fs__stream = new FileStream("ai.dat", FileMode.OpenOrCreate))
                {
                    ai__ai = (c__ai)bf__formatter.Deserialize(fs__stream); // Установить противника из файла
                }

            }
            else
            {
                ai__ai = new c__ai(); // Установить противника	
            }
        }

        

        /**
         * Обучить AI
         * 
         * Проделать большое количество игр с рандомом 
         */
        private void btn_teach_ai_Click(object sender, EventArgs e)
        {
            //
            // Провести 1000 игр с рандомом для обучения AI
            //
            for (int i__1 = 0; i__1 < 1000; i__1++)
            {
                bool boo__game_is_over = false; // Флаг - показыват закончена ли игра

                //
                // Цикл в котором проводится игра
                //
                do
                {
                    try
                    {
                        //
                        // Сделать рандомный ход
                        //
                        for (int i__2 = 0; i__2 < 1; i__2++)
                        {
                            int rn__rand_cell = rnd.Next(0, 9);

                            if (a__b__field[rn__rand_cell] == 0)
                            {
                                a__b__field[rn__rand_cell] = 1;
                                break;
                            }
                        }                        

                        boo__game_is_over = check_game_ending(); // Проверить игру на завершенность


                        //
                        // Если игра еще не завершена
                        // 
                        if (!boo__game_is_over)
                        {
                            a__b__field = ai__ai.make_move(a__b__field); // AI делает ход				


                            boo__game_is_over = check_game_ending(); // Проверить игру на завершенность
                        }

                    }
                    catch (Exception ex__exc)
                    {
                        MessageBox.Show(ex__exc.Message);
                    }
                }
                while (!boo__game_is_over); // Игра продолжается пока она не закончна
            }
        }



        /**
	     * Перерисовать доску на UI
	     */
        public void redraw_ui_board()
        {
            //
            // Изменить значение для каждой кнопки на UI
            //
            for (int i__1 = 0; i__1 < 9; i__1++)
            {
                //
                // Если ячейка не заполнена игроком или AI то 
                // на UI кнопка остается пустой
                //
                if (a__b__field[i__1] != 0)
                {
                    l__btn__ui_field[i__1].Text = a__b__field[i__1].ToString();    // Присвоить кнопке значение
                }
                else
                {
                    l__btn__ui_field[i__1].Text = ""; // Оставить кнопку пустой
                }
            }
        }



        /**
         * Проверить игру на завершенность и указать AI
         * на победу или поражение
         * 
         * @return  bool  Завершенность игры (true - игра завершена)
         */
        public bool check_game_ending()
        {
            //
            // Если выиграл игрок
            //
            if
                (
                    (a__b__field[0] == 1 && a__b__field[1] == 1 && a__b__field[2] == 1) ||
                    (a__b__field[3] == 1 && a__b__field[4] == 1 && a__b__field[5] == 1) ||
                    (a__b__field[6] == 1 && a__b__field[7] == 1 && a__b__field[8] == 1) ||
                    (a__b__field[0] == 1 && a__b__field[3] == 1 && a__b__field[6] == 1) ||
                    (a__b__field[1] == 1 && a__b__field[4] == 1 && a__b__field[7] == 1) ||
                    (a__b__field[2] == 1 && a__b__field[5] == 1 && a__b__field[8] == 1) ||
                    (a__b__field[0] == 1 && a__b__field[4] == 1 && a__b__field[8] == 1) ||
                    (a__b__field[2] == 1 && a__b__field[4] == 1 && a__b__field[6] == 1)
                )
            {
                ai__ai.loose(); // Указать AI, что он проогирал

                a__b__field = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Обнулить доску для игры

                lbl_result.Text = "Player wins"; // Уведомление о победе игрока

                return true; // Игра завершена
            }


            //
            // Если выиграл AI
            //
            if
                (
                    (a__b__field[0] == 2 && a__b__field[1] == 2 && a__b__field[2] == 2) ||
                    (a__b__field[3] == 2 && a__b__field[4] == 2 && a__b__field[5] == 2) ||
                    (a__b__field[6] == 2 && a__b__field[7] == 2 && a__b__field[8] == 2) ||
                    (a__b__field[0] == 2 && a__b__field[3] == 2 && a__b__field[6] == 2) ||
                    (a__b__field[1] == 2 && a__b__field[4] == 2 && a__b__field[7] == 2) ||
                    (a__b__field[2] == 2 && a__b__field[5] == 2 && a__b__field[8] == 2) ||
                    (a__b__field[0] == 2 && a__b__field[4] == 2 && a__b__field[8] == 2) ||
                    (a__b__field[2] == 2 && a__b__field[4] == 2 && a__b__field[6] == 2)
                )
            {
                ai__ai.win(); // Указать AI, что он выиграл

                a__b__field = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Обнулить доску для игры

                lbl_result.Text = "AI wins"; // Уведомление о победе AI

                lbl_wins.Text = ai__ai.get_wins_count().ToString();

                return true; // Игра завершена
            }


            //
            // Если пустых ячеек на доске не осталось, то игра завершается
            //
            if
                (
                    a__b__field[0] != 0 && a__b__field[1] != 0 && a__b__field[2] != 0 &&
                    a__b__field[3] != 0 && a__b__field[4] != 0 && a__b__field[5] != 0 &&
                    a__b__field[6] != 0 && a__b__field[7] != 0 && a__b__field[8] != 0
                )
            {
                a__b__field = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Обнулить доску для игры

                lbl_result.Text = "Standoff"; // Уведомление о ничьей

                return true; // Игра завершена
            }


            lbl_result.Text = ""; // Не выводить уведомление о окончании игры

            return false; // Игра еще не завершена
        }



        //-----------------------------------------------------------------------------------//
        //  Действия на кнопки доски для игры с AI                                           //
        //-----------------------------------------------------------------------------------//

        /**
         * Сделать ход, проверить игру на завершенность, пердать ход противнику
         * 
         * @param  int     i__cell      Порядковый номер (с 0) чейки доски за которую отвечает эта кнопка
         */
        public void player_click(int i__cell)
        {
            try
            {
                bool b__game_is_over = false; // Флаг окончания игры (true = игра закончена)   


                //
                // Если ячейка пуста, то сделать ход
                //
                if (a__b__field[i__cell] == 0)
                {
                    a__b__field[i__cell] = 1; // Сделать ход
                }
                else
                {
                    return; // Если ячейка уже занята, то прервать выполнение метода
                }


                redraw_ui_board(); // Обновить доску на UI


                b__game_is_over = check_game_ending(); // Проверить игру на завершенность


                //
                // Если игра еще не завершена
                // 
                if (!b__game_is_over)
                {
                    a__b__field = ai__ai.make_move(a__b__field); // AI делает ход				

                    redraw_ui_board(); // Обновить доску на UI


                    b__game_is_over = check_game_ending(); // Проверить игру на завершенность
                }

            }
            catch (Exception ex__exc)
            {
                MessageBox.Show(ex__exc.Message);
            }
        }



        private void btn_0_Click(object sender, EventArgs e)
        {
            player_click(0);
        }



        private void btn_1_Click(object sender, EventArgs e)
        {
            player_click(1);
        }



        private void btn_2_Click(object sender, EventArgs e)
        {
            player_click(2);
        }



        private void btn_3_Click(object sender, EventArgs e)
        {
            player_click(3);
        }



        private void btn_4_Click(object sender, EventArgs e)
        {
            player_click(4);
        }



        private void btn_5_Click(object sender, EventArgs e)
        {
            player_click(5);
        }



        private void btn_6_Click(object sender, EventArgs e)
        {
            player_click(6);
        }



        private void btn_7_Click(object sender, EventArgs e)
        {
            player_click(7);
        }



        private void btn_8_Click(object sender, EventArgs e)
        {
            player_click(8);
        }
        //-----------------------------------------------------------------------------------//



        /**
         * Сохранить опыт AI перед закрытием приложения
         */
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //
            // Удалить старый файл записей AI
            //
            File.Delete("ai.dat");


            //
            // Сериализовать классы AI
            //
            BinaryFormatter bf__formatter = new BinaryFormatter(); // Инструмент для сериализации

            //
            // Открыть поток для записи файла
            //
            using (FileStream fs__stream = new FileStream("ai.dat", FileMode.OpenOrCreate))
            {
                bf__formatter.Serialize(fs__stream, ai__ai); // Сериализовать AI
            }
        }
    }
}
