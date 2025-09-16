using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MathGame
{
    public partial class MainPage : ContentPage
    {
       
        private int currentQuestion = 1;
        private int totalScore = 0;
        private int difficultyLevel = 1;
        private bool isAdditionEnabled = false;
        private bool isSubtractionEnabled = false;
        private bool isMultiplicationEnabled = false;
        private bool isDivisionEnabled = false;

        private int timeRemaining = 30; 
        private bool gameInProgress = false;

        private float correctAnswer;
        private DateTime questionStartTime;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void StartGame_Clicked(object sender, EventArgs e)
        {
           
            ConfigLayout.IsVisible = false;
            GameLayout.IsVisible = true;
            gameInProgress = true;

            
            currentQuestion = 1;
            totalScore = 0;
            lbPontos.Text = "0";
            lbQuestao.Text = "1/10";

            GenerateQuestion();
        }

        private void DifficultyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            switch (selectedIndex)
            {
                case 0:
                    difficultyLevel = 1;
                    break;
                case 1:
                    difficultyLevel = 2;
                    break;
                case 2:
                    difficultyLevel = 3;
                    break;
            }
        }

        private void Addition_CheckedChanged(object sender, CheckedChangedEventArgs e) => isAdditionEnabled = e.Value;
        private void Subtraction_CheckedChanged(object sender, CheckedChangedEventArgs e) => isSubtractionEnabled = e.Value;
        private void Multiplication_CheckedChanged(object sender, CheckedChangedEventArgs e) => isMultiplicationEnabled = e.Value;
        private void Division_CheckedChanged(object sender, CheckedChangedEventArgs e) => isDivisionEnabled = e.Value;

        private void GenerateQuestion()
        {
            if (currentQuestion > 10)
            {
                EndGame();
                return;
            }

            
            Random rand = new Random();
            int maxRange = 10;
            switch (difficultyLevel)
            {
                case 2:
                    maxRange = 50;
                    break;
                case 3:
                    maxRange = 100;
                    break;
            }

            int num1 = rand.Next(1, maxRange + 1);
            int num2 = rand.Next(1, maxRange + 1);

            
            var enabledOperations = new System.Collections.Generic.List<char>();
            if (isAdditionEnabled) enabledOperations.Add('+');
            if (isSubtractionEnabled) enabledOperations.Add('-');
            if (isMultiplicationEnabled) enabledOperations.Add('x');
            if (isDivisionEnabled) enabledOperations.Add('÷');

            if (enabledOperations.Count == 0)
            {
                
                enabledOperations.Add('+');
            }

            char operation = enabledOperations[rand.Next(enabledOperations.Count)];

           
            string questionText = "";
            switch (operation)
            {
                case '+':
                    correctAnswer = num1 + num2;
                    questionText = $"{num1} + {num2} = ?";
                    break;
                case '-':
                    correctAnswer = num1 - num2;
                    questionText = $"{num1} - {num2} = ?";
                    break;
                case 'x':
                    correctAnswer = num1 * num2;
                    questionText = $"{num1} x {num2} = ?";
                    break;
                case '÷':
                    num1 = num1 * num2;
                    correctAnswer = num1 / num2;
                    questionText = $"{num1} ÷ {num2} = ?";
                    break;
            }

            lbQuestion.Text = questionText;
            entryResposta.Text = string.Empty;
            lbFeedback.Text = string.Empty;

            questionStartTime = DateTime.Now;
            StartTimer();
        }

        private async void StartTimer()
        {
            timeRemaining = 30;
            lbTempo.Text = $"{timeRemaining}s";
            lbTempo.TextColor = Colors.Orange;

            while (timeRemaining > 0 && gameInProgress)
            {
                await Task.Delay(1000);
                timeRemaining--;
                lbTempo.Text = $"{timeRemaining}s";

                if (timeRemaining <= 10)
                {
                    lbTempo.TextColor = Colors.Red;
                }
            }

            if (gameInProgress)
            {
                CheckAnswer_Clicked(this, EventArgs.Empty);
            }
        }

        private void CheckAnswer_Clicked(object sender, EventArgs e)
        {
            if (!gameInProgress) return;

            float userResponse;
            bool isValid = float.TryParse(entryResposta.Text, out userResponse);

            if (!isValid)
            {
                lbFeedback.Text = "Erro! Digite um número válido.";
                lbFeedback.TextColor = Colors.Red;
                return;
            }

            bool isCorrect = userResponse == correctAnswer;
            UpdateScore(isCorrect);

            
            lbFeedback.Text = isCorrect ? $"Correto! +{CalculatePoints()} pontos" : $"Errado! A resposta era {correctAnswer}";
            lbFeedback.TextColor = isCorrect ? Colors.Green : Colors.Red;

            
            currentQuestion++;
            lbQuestao.Text = $"{currentQuestion}/10";

            
            Task.Delay(1500).ContinueWith(_ => Device.BeginInvokeOnMainThread(GenerateQuestion));
        }

        private int CalculatePoints()
        {
            int basePoints = difficultyLevel * 10;
            double timeBonus = (30 - (DateTime.Now - questionStartTime).TotalSeconds) * 2;
            int total = basePoints + (int)Math.Max(0, timeBonus);
            return total;
        }

        private void UpdateScore(bool isCorrect)
        {
            if (isCorrect)
            {
                int points = CalculatePoints();
                totalScore += points;
            }
            
            lbPontos.Text = totalScore.ToString();
        }

        private void EndGame()
        {
            gameInProgress = false;
            ConfigLayout.IsVisible = true;
            GameLayout.IsVisible = false;
            DisplayAlert("Fim de Jogo", $"Parabéns! Sua pontuação final é: {totalScore}", "OK");
        }
    }
}