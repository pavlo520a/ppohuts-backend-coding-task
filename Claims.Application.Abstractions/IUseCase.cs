namespace Claims.Application.Abstractions;

public interface IUseCase<TCommand> where TCommand : ICommand
{
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}

public interface IUseCase<TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}
