// ============================================================
// Classe: CsvReader
// Responsabilidade: Lê e interpreta o arquivo CSV, devolvendo
// uma lista de objetos Cliente prontos para uso.
// ============================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using AutomacaoCobranca.Models;

namespace AutomacaoCobranca.Services
{
    public class CsvReader
    {
        // Separador de colunas usado no CSV (vírgula ou ponto-e-vírgula)
        private readonly char _separador;

        // Construtor permite configurar o separador (padrão: ponto-e-vírgula)
        public CsvReader(char separador = ';')
        {
            _separador = separador;
        }

        // --------------------------------------------------------
        // Método principal: recebe o caminho do CSV e retorna a lista
        // --------------------------------------------------------
        public List<Cliente> Ler(string caminhoCsv)
        {
            var clientes = new List<Cliente>();

            // Verifica se o arquivo existe antes de tentar abrir
            if (!File.Exists(caminhoCsv))
                throw new FileNotFoundException($"Arquivo CSV não encontrado: {caminhoCsv}");

            // Lê todas as linhas do arquivo (UTF-8 para suportar acentos)
            string[] linhas = File.ReadAllLines(caminhoCsv, Encoding.UTF8);

            Console.WriteLine($"[INFO] {linhas.Length} linha(s) encontrada(s) no CSV (incluindo cabeçalho).");

            // Começa no índice 1 para pular o cabeçalho (linha 0)
            for (int i = 1; i < linhas.Length; i++)
            {
                string linha = linhas[i].Trim();

                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                // Tenta processar a linha; erros não interrompem o programa
                try
                {
                    Cliente cliente = ProcessarLinha(linha, i + 1);
                    clientes.Add(cliente);
                }
                catch (Exception ex)
                {
                    // Avisa sobre a linha problemática e continua processando
                    Console.WriteLine($"[AVISO] Linha {i + 1} ignorada: {ex.Message}");
                }
            }

            Console.WriteLine($"[INFO] {clientes.Count} cliente(s) carregado(s) com sucesso.");
            return clientes;
        }

        // --------------------------------------------------------
        // Método auxiliar: interpreta uma única linha do CSV
        // --------------------------------------------------------
        private Cliente ProcessarLinha(string linha, int numeroLinha)
        {
            // Divide a linha pelo separador configurado
            string[] colunas = linha.Split(_separador);

            // Valida que existem pelo menos 3 colunas
            if (colunas.Length < 3)
                throw new FormatException($"Esperado 3 colunas, encontrado {colunas.Length}.");

            string nome  = colunas[0].Trim();
            string email = colunas[1].Trim();
            string valorBruto = colunas[2].Trim()
                                          .Replace("R$", "")   // remove prefixo monetário
                                          .Replace(" ", "")    // remove espaços
                                          .Trim();

            // Valida campos obrigatórios
            if (string.IsNullOrEmpty(nome))
                throw new FormatException("Campo 'Nome' está vazio.");

            if (string.IsNullOrEmpty(email))
                throw new FormatException("Campo 'Email' está vazio.");

            // Converte o valor para decimal usando cultura invariante
            // Aceita tanto ponto quanto vírgula como separador decimal
            valorBruto = valorBruto.Replace(",", ".");
            if (!decimal.TryParse(valorBruto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
                throw new FormatException($"Valor '{valorBruto}' não é um número válido.");

            return new Cliente(nome, email, valor);
        }
    }
}
