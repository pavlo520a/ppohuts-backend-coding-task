namespace Claims.Application.Abstractions;

public interface IUseCase<TCommand>
{
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}

public interface IUseCase<TCommand, TResult>
{
    Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ISyncUseCase<TCommand, TResult>
{
    TResult Execute(TCommand command);
}
