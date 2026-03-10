// ============================================================
// Classe: Relatorio
// Responsabilidade: Recebe uma lista de clientes, filtra os
// inadimplentes e gera o relatório em arquivo .TXT.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutomacaoCobranca.Models;

namespace AutomacaoCobranca.Services
{
    public class Relatorio
    {
        // Lista interna com TODOS os clientes (com e sem pendência)
        private readonly List<Cliente> _todosClientes;

        // Lista filtrada: apenas clientes COM valor pendente > 0
        private readonly List<Cliente> _clientesInadimplentes;

        // Construtor recebe a lista completa e já aplica o filtro
        public Relatorio(List<Cliente> clientes)
        {
            _todosClientes = clientes;

            // LINQ: filtra e ordena pelo valor pendente (maior primeiro)
            _clientesInadimplentes = clientes
                .Where(c => c.TemPendencia)
                .OrderByDescending(c => c.ValorPendente)
                .ToList();
        }

        // --------------------------------------------------------
        // Propriedades calculadas (lidas a partir da lista filtrada)
        // --------------------------------------------------------

        // Quantidade de clientes inadimplentes
        public int QuantidadeInadimplentes => _clientesInadimplentes.Count;

        // Soma de todos os valores pendentes
        public decimal TotalPendente => _clientesInadimplentes.Sum(c => c.ValorPendente);

        // --------------------------------------------------------
        // Método principal: monta o conteúdo e salva o arquivo TXT
        // --------------------------------------------------------
        public void GerarArquivoTxt(string caminhoSaida)
        {
            // StringBuilder é mais eficiente do que concatenar strings em loop
            var sb = new StringBuilder();

            // --- Cabeçalho do relatório ---
            sb.AppendLine("=============================================================");
            sb.AppendLine("           RELATÓRIO DE CLIENTES COM PAGAMENTO PENDENTE      ");
            sb.AppendLine("=============================================================");
            sb.AppendLine($"  Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("=============================================================");
            sb.AppendLine();

            // --- Listagem dos clientes inadimplentes ---
            if (_clientesInadimplentes.Count == 0)
            {
                sb.AppendLine("  Nenhum cliente com pagamento pendente encontrado.");
            }
            else
            {
                sb.AppendLine("  CLIENTES COM PENDÊNCIAS:");
                sb.AppendLine(new string('-', 61));

                // Itera cada cliente e formata a linha de detalhe
                for (int i = 0; i < _clientesInadimplentes.Count; i++)
                {
                    var c = _clientesInadimplentes[i];
                    sb.AppendLine($"  {i + 1,3}. {c.Nome,-30} | {c.Email,-30}");
                    sb.AppendLine($"       Valor Pendente: R$ {c.ValorPendente,10:F2}");
                    sb.AppendLine(new string('-', 61));
                }
            }

            sb.AppendLine();

            // --- Rodapé com totalizadores ---
            sb.AppendLine("=============================================================");
            sb.AppendLine("  RESUMO");
            sb.AppendLine("=============================================================");
            sb.AppendLine($"  Total de clientes no arquivo  : {_todosClientes.Count,5}");
            sb.AppendLine($"  Clientes com pendência        : {QuantidadeInadimplentes,5}");
            sb.AppendLine($"  Clientes em dia               : {_todosClientes.Count - QuantidadeInadimplentes,5}");
            sb.AppendLine($"  Valor total pendente          : R$ {TotalPendente,10:F2}");
            sb.AppendLine("=============================================================");

            // --- Gravação do arquivo ---
            // Encoding UTF-8 com BOM para garantir acentos no Windows
            File.WriteAllText(caminhoSaida, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            Console.WriteLine($"[OK] Relatório salvo em: {caminhoSaida}");
        }
    }
}
