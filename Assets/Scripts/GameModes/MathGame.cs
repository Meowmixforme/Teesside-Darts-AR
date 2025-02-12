using UnityEngine;
using System.Collections.Generic;

public class MathGame : BaseDartGame
{
    private int currentTarget;
    private int questionsAnswered;
    private int correctAnswers;
    private string currentQuestion;
    private const int TOTAL_QUESTIONS = 10;

    // Create a struct for our math problems
    private struct MathProblem
    {
        public string Question;
        public int Answer;

        public MathProblem(string question, int answer)
        {
            Question = question;
            Answer = answer;
        }
    }

    // Use the struct instead of tuples
    private readonly List<MathProblem> mathProblems = new List<MathProblem>
    {
        new MathProblem("7 + 13 = ?", 20),
        new MathProblem("25 - 8 = ?", 17),
        new MathProblem("3 × 6 = ?", 18),
        new MathProblem("48 ÷ 3 = ?", 16),
        new MathProblem("9 + 6 = ?", 15),
        new MathProblem("4 × 5 = ?", 20),
        new MathProblem("21 - 2 = ?", 19),
        new MathProblem("7 × 2 = ?", 14),
        new MathProblem("36 ÷ 3 = ?", 12),
        new MathProblem("8 + 8 = ?", 16)
    };

    private List<MathProblem> currentGameQuestions;

    public override void InitializeGame()
    {
        questionsAnswered = 0;
        correctAnswers = 0;
        totalThrowsLeft = GetMaxThrows();
        throwsLeft = 3;

        // Randomly select questions for this game
        currentGameQuestions = new List<MathProblem>();
        var tempQuestions = new List<MathProblem>(mathProblems);

        for (int i = 0; i < TOTAL_QUESTIONS && tempQuestions.Count > 0; i++)
        {
            int index = Random.Range(0, tempQuestions.Count);
            currentGameQuestions.Add(tempQuestions[index]);
            tempQuestions.RemoveAt(index);
        }

        SetNextQuestion();
    }

    private void SetNextQuestion()
    {
        if (questionsAnswered < TOTAL_QUESTIONS)
        {
            var problem = currentGameQuestions[questionsAnswered];
            currentQuestion = problem.Question;
            currentTarget = problem.Answer;
        }
    }

    public override bool ProcessScore(int baseNumber, int multiplier)
    {
        throwsLeft--;
        totalThrowsLeft--;

        if (baseNumber == currentTarget)
        {
            correctAnswers++;
            questionsAnswered++;
            SetNextQuestion();
            return true;
        }
        else
        {
            questionsAnswered++;
            SetNextQuestion();
            return false;
        }
    }

    public override bool CheckWinCondition()
    {
        return questionsAnswered >= TOTAL_QUESTIONS;
    }

    public override string GetScoreDisplay()
    {
        if (questionsAnswered >= TOTAL_QUESTIONS)
        {
            return $"Game Over!\nYou got {correctAnswers} out of {TOTAL_QUESTIONS} correct!";
        }

        return $"Question {questionsAnswered + 1}/{TOTAL_QUESTIONS}:\n{currentQuestion}\nCorrect so far: {correctAnswers}";
    }

    public override int GetMaxThrows()
    {
        return TOTAL_QUESTIONS * 3; // 3 throws per question
    }

    public string GetCurrentQuestion()
    {
        return currentQuestion;
    }

    public int GetCurrentAnswer()
    {
        return currentTarget;
    }
}