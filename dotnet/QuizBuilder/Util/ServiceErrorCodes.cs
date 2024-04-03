namespace QuizBuilder.Util
{
    public enum ServiceErrorCodes
    {
        NotEntityOwner,
        ValueOutOfRange,
        CannotAnswerOwnQuiz,
        QuizNotPublished,
        QuizAlreadyAnswered,
        TooManyQuestionAnswers,
        CannotReadYourResponses,
        CannotUpdatePublishedQuiz,
        LoginFailed,
        EntityNotFound,
        CreateUserFailed,
        FailedToFetchEntity
    }
}
