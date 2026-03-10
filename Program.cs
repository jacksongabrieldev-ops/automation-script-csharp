// ============================================================
// Arquivo: Program.cs
// Ponto de entrada da aplicação de automação de cobrança.
// Orquestra a leitura do CSV e a geração do relatório TXT.
// ============================================================

using System;
using System.IO;
using AutomacaoCobranca.Services;

namespace AutomacaoCobranca
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=============================================================");
            Console.WriteLine("        SISTEMA DE AUTOMAÇÃO DE COBRANÇA - v1.0             ");
            Console.WriteLine("=============================================================");
            Console.WriteLine();

            // --------------------------------------------------------
            // 1. Define os caminhos de entrada e saída
            //    Por padrão usa a pasta onde o executável está,
            //    mas aceita argumentos de linha de comando:
            //    AutomacaoCobranca.exe clientes.csv relatorio.txt
            // --------------------------------------------------------
            string diretorioBase = AppDomain.CurrentDomain.BaseDirectory;

            string arquivoCsv = args.Length > 0
                ? args[0]
                : Path.Combine(diretorioBase, "clientes.csv");

            string arquivoRelatorio = args.Length > 1
                ? args[1]
                : Path.Combine(diretorioBase, "relatorio.txt");

            Console.WriteLine($"[INFO] Arquivo CSV    : {arquivoCsv}");
            Console.WriteLine($"[INFO] Arquivo saída  : {arquivoRelatorio}");
            Console.WriteLine();

            try
            {
                // --------------------------------------------------------
                // 2. Lê o CSV usando o serviço CsvReader
                //    Separador padrão: ponto-e-vírgula (;)
                //    Troque para ',' se o seu CSV usar vírgula
                // --------------------------------------------------------
                var leitor = new CsvReader(separador: ';');
                var clientes = leitor.Ler(arquivoCsv);

                if (clientes.Count == 0)
                {
                    Console.WriteLine("[AVISO] Nenhum cliente foi carregado. Verifique o CSV.");
                    return;
                }

                // --------------------------------------------------------
                // 3. Gera o relatório TXT usando a classe Relatorio
                // --------------------------------------------------------
                var relatorio = new Relatorio(clientes);
                relatorio.GerarArquivoTxt(arquivoRelatorio);

                // --------------------------------------------------------
                // 4. Exibe resumo no console para confirmação imediata
                // --------------------------------------------------------
                Console.WriteLine();
                Console.WriteLine("--- RESUMO NO CONSOLE ---");
                Console.WriteLine($"  Clientes com pendência : {relatorio.QuantidadeInadimplentes}");
                Console.WriteLine($"  Total pendente         : R$ {relatorio.TotalPendente:F2}");
                Console.WriteLine();
                Console.WriteLine("Processo concluído com sucesso!");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[ERRO] Arquivo não encontrado: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha inesperada: {ex.Message}");
            }

            Console.WriteLine();
            Console.Write("Pressione ENTER para sair...");
            Console.ReadLine();
        }
    }
}
