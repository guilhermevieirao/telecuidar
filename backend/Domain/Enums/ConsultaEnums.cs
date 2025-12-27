namespace Domain.Enums;

public enum StatusConsulta
{
    Agendada,
    Confirmada,
    EmAndamento,
    Concluida,
    Cancelada,
    Realizada,
    NaoRealizada
}

public enum TipoConsulta
{
    PrimeiraVez,
    Retorno,
    Rotina,
    Urgencia,
    Comum,
    Encaminhamento,
    Teleconsulta
}
