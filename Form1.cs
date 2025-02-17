using System;
using System.Threading;
using System.Windows.Forms;

namespace DiningPhilosophers
{
    public partial class Form1 : Form
    {
        private const int PhilosopherCount = 5;
        private Semaphore waiter = new Semaphore(4, 4); 
        private Mutex[] forks = new Mutex[PhilosopherCount];
        private Thread[] philosopherThreads = new Thread[PhilosopherCount];
        private int[] mealsCount = new int[PhilosopherCount];

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < PhilosopherCount; i++)
            {
                forks[i] = new Mutex();
            }

            for (int i = 0; i < PhilosopherCount; i++)
            {
                int philosopherIndex = i;
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
            UpdatePhilosopherState(philosopherIndex, "Thinking...");
            Thread.Sleep( new Random().Next(1000, 5000));
        }

        private void Eat(int philosopherIndex)
        {
            int leftFork = philosopherIndex;
            int rightFork = (philosopherIndex + 1) % PhilosopherCount;

            waiter.WaitOne();

            bool gotLeftFork = false;
            bool gotRightFork = false;

            try
            {
                gotLeftFork = forks[leftFork].WaitOne();
                if (gotLeftFork)
                {
                    gotRightFork = forks[rightFork].WaitOne(1000);
                }

                if (gotLeftFork && gotRightFork)
                {
                    UpdateForkState(leftFork, "Taken");
                    UpdateForkState(rightFork, "Taken");

                    UpdatePhilosopherState(philosopherIndex, "Eating...");
                    mealsCount[philosopherIndex]++;
                    UpdateMealsCount(philosopherIndex);

                    Thread.Sleep(new Random().Next(1000, 5000)); 

                    forks[leftFork].ReleaseMutex();
                    forks[rightFork].ReleaseMutex(); 

                    UpdateForkState(leftFork, "Free");
                    UpdateForkState(rightFork, "Free");
                }
            }
            finally
            {
                if (gotLeftFork && !gotRightFork)
                {
                    forks[leftFork].ReleaseMutex();
                }
            }

            waiter.Release();
        }

        private void UpdatePhilosopherState(int philosopherIndex, string state)
        {
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
