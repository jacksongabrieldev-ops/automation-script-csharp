// ============================================================
// Classe: Cliente
// Responsabilidade: Representa um cliente lido do arquivo CSV.
// Cada instância armazena os dados de uma linha do CSV.
// ============================================================

namespace AutomacaoCobranca.Models
{
    public class Cliente
    {
        // Nome completo do cliente
        public string Nome { get; set; }

        // Endereço de e-mail do cliente
        public string Email { get; set; }

        // Valor financeiro pendente do cliente (pode ser zero)
        public decimal ValorPendente { get; set; }

        // Propriedade calculada: retorna true se o cliente tem débito
        public bool TemPendencia => ValorPendente > 0;

        // Construtor principal que recebe todos os campos
        public Cliente(string nome, string email, decimal valorPendente)
        {
            Nome = nome;
            Email = email;
            ValorPendente = valorPendente;
        }

        // Sobrescreve ToString para facilitar debug e leitura no relatório
        public override string ToString()
        {
            return $"Nome: {Nome} | Email: {Email} | Pendente: R$ {ValorPendente:F2}";
        }
    }
}
