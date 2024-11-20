using System;
using System.Threading;
using System.Windows.Forms;

namespace DiningPhilosophers
{
    public partial class Form1 : Form
    {
        private const int PhilosopherCount = 5;
        private Semaphore waiter = new Semaphore(4, 4); // Семафор для предотвращения дедлока
        private Mutex[] forks = new Mutex[PhilosopherCount]; // Массив мьютексов для вилок
        private Thread[] philosopherThreads = new Thread[PhilosopherCount];
        private int[] mealsCount = new int[PhilosopherCount]; // Количество приемов пищи для каждого философа

        public Form1()
        {
            InitializeComponent();
            // Инициализируем мьютексы для вилок
            for (int i = 0; i < PhilosopherCount; i++)
            {
                forks[i] = new Mutex(); // Каждый философ получает мьютекс для своей вилки
            }

            // Создаем потоки для философов
            for (int i = 0; i < PhilosopherCount; i++)
            {
                int philosopherIndex = i; // Локальная переменная для замыкания в потоке
                philosopherThreads[i] = new Thread(() => Philosopher(philosopherIndex));
                philosopherThreads[i].Start();
            }
        }

        private void Philosopher(int philosopherIndex)
        {
            while (true)
            {
                Think(philosopherIndex);
                Eat(philosopherIndex);
            }
        }

        private void Think(int philosopherIndex)
        {
            // Философ думает
            UpdatePhilosopherState(philosopherIndex, "Thinking...");
            Thread.Sleep( new Random().Next(1000, 5000)); // Философ думает случайное время
        }

        private void Eat(int philosopherIndex)
        {
            int leftFork = philosopherIndex;
            int rightFork = (philosopherIndex + 1) % PhilosopherCount;

            waiter.WaitOne(); // Ожидаем, чтобы избежать дедлока

            // Попытка взять вилки
            bool gotLeftFork = false;
            bool gotRightFork = false;

            try
            {
                // Философ пытается взять левую вилку
                gotLeftFork = forks[leftFork].WaitOne(1000); // Ожидаем мьютекс на вилку с тайм-аутом
                if (gotLeftFork)
                {
                    // Философ пытается взять правую вилку
                    gotRightFork = forks[rightFork].WaitOne(1000); // Ожидаем мьютекс на вилку с тайм-аутом
                }

                if (gotLeftFork && gotRightFork)
                {
                    // Если обе вилки захвачены, философ ест
                    UpdateForkState(leftFork, "Taken");
                    UpdateForkState(rightFork, "Taken");

                    UpdatePhilosopherState(philosopherIndex, "Eating...");
                    mealsCount[philosopherIndex]++;
                    UpdateMealsCount(philosopherIndex);

                    Thread.Sleep(new Random().Next(1000, 5000)); // Философ ест случайное время

                    // Философ закончил есть, кладет вилки
                    forks[leftFork].ReleaseMutex(); // Освобождаем левую вилку
                    forks[rightFork].ReleaseMutex(); // Освобождаем правую вилку

                    UpdateForkState(leftFork, "Free");
                    UpdateForkState(rightFork, "Free");
                }
            }
            finally
            {
                // Если философ смог захватить только одну вилку, освобождаем захваченную
                if (gotLeftFork && !gotRightFork)
                {
                    forks[leftFork].ReleaseMutex(); // Освобождаем левую вилку
                }
                // Если философ не смог захватить обе вилки, он будет снова пытаться
            }

            waiter.Release(); // Освобождаем семафор
        }

        private void UpdatePhilosopherState(int philosopherIndex, string state)
        {
            // Обновляем состояние философа
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    switch (philosopherIndex)
                    {
                        case 0: philosopherLabel1.Text = $"{state}"; break;
                        case 1: philosopherLabel2.Text = $"{state}"; break;
                        case 2: philosopherLabel3.Text = $"{state}"; break;
                        case 3: philosopherLabel4.Text = $"{state}"; break;
                        case 4: philosopherLabel5.Text = $"{state}"; break;
                    }
                }));
            }
        }

        private void UpdateForkState(int forkIndex, string state)
        {
            // Обновляем состояние вилки
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    switch (forkIndex)
                    {
                        case 0: forkLabel1.Text = $"{state}"; break;
                        case 1: forkLabel2.Text = $"{state}"; break;
                        case 2: forkLabel3.Text = $"{state}"; break;
                        case 3: forkLabel4.Text = $"{state}"; break;
                        case 4: forkLabel5.Text = $"{state}"; break;
                    }
                }));
            }
        }

        private void UpdateMealsCount(int philosopherIndex)
        {
            // Обновляем количество приемов пищи
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    switch (philosopherIndex)
                    {
                        case 0: mealsCountLabel1.Text = $"Meals: {mealsCount[0]}"; break;
                        case 1: mealsCountLabel2.Text = $"Meals: {mealsCount[1]}"; break;
                        case 2: mealsCountLabel3.Text = $"Meals: {mealsCount[2]}"; break;
                        case 3: mealsCountLabel4.Text = $"Meals: {mealsCount[3]}"; break;
                        case 4: mealsCountLabel5.Text = $"Meals: {mealsCount[4]}"; break;
                    }
                }));
            }
        }
    }
}
