namespace Claims.Application.Abstractions;

public interface IUseCase<in TCommand>
{
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}

public interface IUseCase<in TCommand, TResult>
{
    Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}
