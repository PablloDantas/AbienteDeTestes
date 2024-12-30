using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ConsoleTables;

namespace AmbienteDeTestes
{
    /// <summary>
    /// Programa que distribui circuitos elétricos entre três fases (R, S, T),
    /// garantindo equilíbrio nas cargas atribuídas a cada fase.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Ponto de entrada principal do programa.
        /// </summary>
        /// <param name="args">Argumentos da linha de comando.</param>
        static void Main(string[] args)
        {
            // Inicializa circuitos com suas cargas e número de fases
            Circuito chu1 = new Circuito("Chuveiro 01", 6000, 2);
            Circuito chu2 = new Circuito("Chuveiro 02", 5000, 2);
            Circuito tug = new Circuito("Tomada de Uso Geral", 5000, 1);
            Circuito tue = new Circuito("Tomada de Uso Específico", 3000, 1);
            Circuito ilu = new Circuito("Iluminação", 2000, 1);
            Circuito mb = new Circuito("Motobomba", 6000, 3);

            // Adiciona os circuitos a uma lista para gerenciamento
            List<Circuito> circuitos = new List<Circuito> { chu1, chu2, tue, tug, ilu, mb };

            // Cria um quadro elétrico ("QD1") e distribui as fases entre os circuitos
            Quadro qd1 = new Quadro("QD1", circuitos);

            // Balanceia as cargas entre as fases R, S e T
            qd1.BalancearCargas();

            // Exibe a tabela de distribuição de circuitos em uma janela utilizando Windows Forms
            qd1.ExibirTabelaEmJanela();

            // Alternativamente, para exibir no console, descomente a linha abaixo:
            // qd1.ExibirTabela();
        }
    }

    /// <summary>
    /// Representa um circuito elétrico com: nome, carga total e quantidade de fases.
    /// </summary>
    public class Circuito
    {
        /// <summary>
        /// Nome do circuito.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Carga total do circuito em watts.
        /// </summary>
        public int CargaTotal { get; set; }

        /// <summary>
        /// Número de fases que o circuito utiliza.
        /// </summary>
        public int QuantidadeDeFases { get; set; }

        // Lista privada que armazena as cargas distribuídas entre as fases
        private List<int> _listaDeCargas;

        /// <summary>
        /// Obtém a lista de cargas distribuídas entre as fases do circuito.
        /// Calcula a carga por fase se ainda não estiver calculada.
        /// </summary>
        public List<int> ListaDeCargas
        {
            get
            {
                if (_listaDeCargas == null)
                {
                    _listaDeCargas = new List<int>();

                    // Calcula a carga por fase dividindo a carga total pelo número de fases
                    int cargaPorFase = CargaTotal / QuantidadeDeFases;

                    // Adiciona a carga calculada para cada fase
                    for (int i = 0; i < QuantidadeDeFases; i++)
                        _listaDeCargas.Add(cargaPorFase);
                }

                return _listaDeCargas;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Circuito"/>.
        /// </summary>
        /// <param name="nome">Nome do circuito.</param>
        /// <param name="cargaTotal">Carga total em watts.</param>
        /// <param name="quantidadeDeFases">Número de fases do circuito.</param>
        public Circuito(string nome, int cargaTotal, int quantidadeDeFases)
        {
            Nome = nome;
            CargaTotal = cargaTotal;
            QuantidadeDeFases = quantidadeDeFases;
        }
    }

    /// <summary>
    /// Representa um quadro elétrico que gerencia a distribuição de circuitos entre as fases R, S e T.
    /// </summary>
    public class Quadro
    {
        /// <summary>
        /// Nome do quadro elétrico.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Lista de circuitos elétricos associados a este quadro.
        /// </summary>
        public List<Circuito> Circuitos { get; set; }

        /// <summary>
        /// Tabela que armazena a distribuição dos circuitos nas fases.
        /// </summary>
        public DataTable TabelaDeCircuitos { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Quadro"/>.
        /// Configura as colunas da tabela que armazenará a distribuição dos circuitos.
        /// </summary>
        /// <param name="nome">Nome do quadro.</param>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos no quadro.</param>
        public Quadro(string nome, List<Circuito> circuitos)
        {
            Nome = nome;
            Circuitos = circuitos;

            // Configura as colunas da tabela de circuitos
            TabelaDeCircuitos = new DataTable
            {
                Columns =
                {
                    new DataColumn("Nome", typeof(string)), // Nome do circuito
                    new DataColumn("Circuito", typeof(Circuito)) { AllowDBNull = true }, // Objeto Circuito
                    new DataColumn("R", typeof(int)), // Carga na fase R
                    new DataColumn("S", typeof(int)), // Carga na fase S
                    new DataColumn("T", typeof(int)), // Carga na fase T
                }
            };
        }

        /// <summary>
        /// Balanceia as cargas dos circuitos entre as fases R, S e T.
        /// Calcula diferentes distribuições e seleciona a que apresenta a menor diferença de carga entre as fases.
        /// </summary>
        public void BalancearCargas()
        {
            // Distribui os circuitos sem ordenação específica
            DataTable tabelaCircuitos = this.DistribuirCircuitos(this.Circuitos);

            // Distribui os circuitos ordenando de forma decrescente (por número de fases e carga)
            DataTable tabelaCircuitosDecrescente = this.DistribuirCircuitosDecrescente(
                this.Circuitos
            );

            // Distribui os circuitos ordenando de forma crescente (por número de fases e carga)
            DataTable tabelaCircuitosCrescente = this.DistribuirCircuitosCrescente(this.Circuitos);

            // Distribui os circuitos de forma aleatória
            DataTable tabelaCircuitosAleatorios = this.DistribuirCircuitosAleatoriamente(
                this.Circuitos
            );

            // Calcula a diferença de carga para cada distribuição
            int amplitudeCircuitos = this.DiferençaDeCarga(tabelaCircuitos);
            int amplitudeCircuitosDecrescente = this.DiferençaDeCarga(tabelaCircuitosDecrescente);
            int amplitudeCircuitosCrescente = this.DiferençaDeCarga(tabelaCircuitosCrescente);
            int amplitudeCircuitosAleatorios = this.DiferençaDeCarga(tabelaCircuitosAleatorios);

            // Lista de todas as tabelas de distribuição
            List<DataTable> tabelas = new List<DataTable>
            {
                tabelaCircuitos,
                tabelaCircuitosDecrescente,
                tabelaCircuitosCrescente,
                tabelaCircuitosAleatorios
            };

            // Lista das diferenças de carga correspondentes
            List<int> amplitudes = new List<int>
            {
                amplitudeCircuitos,
                amplitudeCircuitosDecrescente,
                amplitudeCircuitosCrescente,
                amplitudeCircuitosAleatorios
            };

            // Identifica a tabela com a menor diferença de carga
            int menorAmplitude = amplitudes.IndexOf(amplitudes.Min());

            // Seleciona a tabela balanceada
            DataTable tabelaBalanceada = tabelas[menorAmplitude];

            // Atualiza a tabela de circuitos do quadro com a distribuição balanceada
            this.TabelaDeCircuitos = tabelaBalanceada;
        }

        /// <summary>
        /// Distribui os circuitos elétricos entre as fases R, S e T sem ordenação específica.
        /// </summary>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos.</param>
        /// <returns>DataTable contendo a distribuição dos circuitos nas fases.</returns>
        public DataTable DistribuirCircuitos(List<Circuito> circuitos)
        {
            // Inicializa listas auxiliares para armazenar cargas em cada fase
            List<int> faseR = new List<int>();
            List<int> faseS = new List<int>();
            List<int> faseT = new List<int>();

            // Lista para armazenar circuitos já distribuídos
            List<Circuito> circuitosDistribuidos = new List<Circuito>();

            // Lista para armazenar os nomes dos circuitos
            List<string> nomesCircuitos = new List<string>();

            // Agrupa as fases em uma lista para facilitar o gerenciamento
            List<List<int>> fases = new List<List<int>> { faseR, faseS, faseT };

            // Itera sobre cada circuito para distribuí-lo nas fases
            foreach (var circuito in circuitos)
            {
                // Obtém a lista de cargas distribuídas nas fases para este circuito
                List<int> listaDeCargasCircuito = circuito.ListaDeCargas;

                // Distribui circuitos trifásicos (utilizam todas as três fases)
                if (listaDeCargasCircuito.Count == 3)
                {
                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                    faseR.Add(listaDeCargasCircuito[0]); // Adiciona a carga na fase R
                    faseS.Add(listaDeCargasCircuito[1]); // Adiciona a carga na fase S
                    faseT.Add(listaDeCargasCircuito[2]); // Adiciona a carga na fase T
                }
                // Distribui circuitos bifásicos (utilizam duas fases)
                else if (listaDeCargasCircuito.Count == 2)
                {
                    // Lista para controlar quais fases já foram usadas para este circuito
                    var fasesUtilizadas = new List<List<int>>();

                    // Itera sobre cada carga do circuito bifásico
                    for (int j = 0; j < listaDeCargasCircuito.Count; j++)
                    {
                        int carga = listaDeCargasCircuito[j]; // Carga atual do circuito

                        // Seleciona a fase com a menor soma de carga que ainda não foi usada para este circuito
                        var faseMenorCarga = fases
                            .Where(lista => !fasesUtilizadas.Contains(lista))
                            .MinBy(lista => lista.Sum());

                        // Adiciona a carga à fase selecionada
                        faseMenorCarga.Add(carga);

                        // Marca a fase como utilizada para evitar reutilização
                        fasesUtilizadas.Add(faseMenorCarga);
                    }

                    // Identifica a fase que não foi utilizada e adiciona uma carga zero para ela
                    var faseNaoUtilizada = fases.First(lista => !fasesUtilizadas.Contains(lista));
                    faseNaoUtilizada.Add(0);

                    // Adiciona o circuito à lista de distribuídos e seu nome à lista de nomes
                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
                // Distribui circuitos monofásicos (utilizam apenas uma fase)
                else if (listaDeCargasCircuito.Count == 1)
                {
                    int carga = listaDeCargasCircuito[0]; // Carga do circuito monofásico

                    // Seleciona a fase com a menor soma de carga para balancear
                    var faseMenorCarga = fases.MinBy(lista => lista.Sum());

                    // Adiciona a carga à fase selecionada
                    faseMenorCarga.Add(carga);

                    // Preenche as outras fases com carga zero para este circuito
                    var fasesNaoUtilizadas = fases.Where(lista => lista != faseMenorCarga).ToList();

                    foreach (var fase in fasesNaoUtilizadas)
                        fase.Add(0);

                    // Adiciona o circuito à lista de distribuídos e seu nome à lista de nomes
                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
            }

            // Gera e retorna a tabela de distribuição com os dados coletados
            return GerarTabela(nomesCircuitos, circuitosDistribuidos, faseR, faseS, faseT);
        }

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T, ordenando-os por número de fases e carga total de forma decrescente.
        /// Isso prioriza circuitos que utilizam mais fases e têm maior carga, ajudando no balanceamento.
        /// </summary>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos.</param>
        /// <returns>DataTable contendo a distribuição dos circuitos nas fases.</returns>
        public DataTable DistribuirCircuitosDecrescente(List<Circuito> circuitos)
        {
            // Ordena os circuitos primeiro pelo número de fases (decrescente) e depois pela soma das cargas (decrescente)
            var circuitosOrdenados = circuitos
                .OrderByDescending(x => x.ListaDeCargas.Count)
                .ThenByDescending(x => x.ListaDeCargas.Sum())
                .ToList();

            // Distribui os circuitos ordenados nas fases
            return DistribuirCircuitos(circuitosOrdenados);
        }

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T, ordenando-os por número de fases e carga total de forma crescente.
        /// Isso prioriza circuitos que utilizam menos fases e têm menor carga.
        /// </summary>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos.</param>
        /// <returns>DataTable contendo a distribuição dos circuitos nas fases.</returns>
        public DataTable DistribuirCircuitosCrescente(List<Circuito> circuitos)
        {
            // Ordena os circuitos primeiro pelo número de fases (crescente) e depois pela soma das cargas (crescente)
            var circuitosOrdenados = circuitos
                .OrderBy(x => x.ListaDeCargas.Count)
                .ThenBy(x => x.ListaDeCargas.Sum())
                .ToList();

            // Distribui os circuitos ordenados nas fases
            return DistribuirCircuitos(circuitosOrdenados);
        }

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T de forma aleatória, buscando equilibrar as cargas.
        /// </summary>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos.</param>
        /// <returns>DataTable contendo a distribuição dos circuitos nas fases.</returns>
        public DataTable DistribuirCircuitosAleatoriamente(List<Circuito> circuitos)
        {
            Random random = new Random();

            // Ordena os circuitos de forma aleatória e, em seguida, por soma das cargas (decrescente)
            var circuitosOrdenados = circuitos
                .OrderBy(x => random.Next())
                .ThenByDescending(x => x.ListaDeCargas.Sum())
                .ToList();

            // Distribui os circuitos ordenados nas fases
            return DistribuirCircuitos(circuitosOrdenados);
        }

        /// <summary>
        /// Calcula a carga total de cada fase (R, S, T) a partir de uma tabela de cargas.
        /// </summary>
        /// <param name="tabelaDeCargas">DataTable contendo as cargas distribuídas nas fases.</param>
        /// <returns>Lista com as cargas totais das fases R, S e T, respectivamente.</returns>
        public List<int> CalcularCargaTotal(DataTable tabelaDeCargas)
        {
            // Calcula os totais das colunas R, S e T utilizando LINQ
            int totalR = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("R"));
            int totalS = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("S"));
            int totalT = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("T"));

            // Retorna a soma das cargas por fase
            List<int> fasesSomadas = new List<int> { totalR, totalS, totalT };

            return fasesSomadas;
        }

        /// <summary>
        /// Calcula a diferença de carga entre as fases, definida como a diferença entre a maior e a menor carga total.
        /// </summary>
        /// <param name="tabelaDeCargas">DataTable contendo as cargas distribuídas nas fases.</param>
        /// <returns>Diferença de carga entre as fases (maior carga - menor carga).</returns>
        public int DiferençaDeCarga(DataTable tabelaDeCargas)
        {
            // Obtém as cargas totais de cada fase
            List<int> cargasTotais = CalcularCargaTotal(tabelaDeCargas);

            // Identifica a maior e a menor carga total
            int maior = cargasTotais.Max();
            int menor = cargasTotais.Min();

            // Retorna a diferença entre a maior e a menor carga
            return maior - menor;
        }

        /// <summary>
        /// Gera uma tabela de distribuição de circuitos nas fases R, S e T.
        /// </summary>
        /// <param name="nomesDosCircuitos">Lista com os nomes dos circuitos.</param>
        /// <param name="circuitos">Lista com os objetos Circuito distribuídos.</param>
        /// <param name="faseR">Lista de cargas na fase R.</param>
        /// <param name="faseS">Lista de cargas na fase S.</param>
        /// <param name="faseT">Lista de cargas na fase T.</param>
        /// <returns>DataTable contendo a distribuição dos circuitos nas fases.</returns>
        public DataTable GerarTabela(
            List<string> nomesDosCircuitos,
            List<Circuito> circuitos,
            List<int> faseR,
            List<int> faseS,
            List<int> faseT
        )
        {
            // Cria uma nova tabela com as colunas especificadas
            DataTable tabelaDeCircuitos = new DataTable
            {
                Columns =
                {
                    new DataColumn("Nome", typeof(string)), // Nome do circuito
                    new DataColumn("Circuito", typeof(Circuito)) { AllowDBNull = true }, // Objeto Circuito
                    new DataColumn("R", typeof(int)), // Carga na fase R
                    new DataColumn("S", typeof(int)), // Carga na fase S
                    new DataColumn("T", typeof(int)), // Carga na fase T
                }
            };

            // Preenche as linhas da tabela com os dados dos circuitos distribuídos
            for (int i = 0; i < circuitos.Count; i++)
            {
                tabelaDeCircuitos.Rows.Add(
                    nomesDosCircuitos[i], // Nome do circuito
                    circuitos[i], // Objeto Circuito
                    faseR[i], // Carga na fase R
                    faseS[i], // Carga na fase S
                    faseT[i] // Carga na fase T
                );
            }

            return tabelaDeCircuitos;
        }

        /// <summary>
        /// Exibe a tabela de distribuição de circuitos no console utilizando a biblioteca ConsoleTables.
        /// </summary>
        public void ExibirTabela()
        {
            // Cria uma instância de ConsoleTable para formatação
            var consoleTable = new ConsoleTable();

            // Adiciona os nomes das colunas à tabela
            foreach (DataColumn coluna in TabelaDeCircuitos.Columns)
            {
                consoleTable.AddColumn(new[] { coluna.ColumnName });
            }

            // Adiciona as linhas da tabela com os dados dos circuitos
            foreach (DataRow row in TabelaDeCircuitos.Rows)
            {
                // Converte objetos DBNull para strings vazias para melhor exibição
                var itens = row
                    .ItemArray.Select(item => item == DBNull.Value ? "" : item.ToString())
                    .ToArray();
                consoleTable.AddRow(itens);
            }

            // Calcula os totais das colunas R, S e T
            int totalR = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("R"));
            int totalS = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("S"));
            int totalT = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("T"));

            // Adiciona a linha de totais à tabela
            consoleTable.AddRow("Totais", "", totalR, totalS, totalT);

            // Imprime a tabela formatada no console
            consoleTable.Write(Format.Alternative);
        }

        /// <summary>
        /// Exibe a tabela de distribuição de circuitos em uma janela utilizando Windows Forms.
        /// </summary>
        public void ExibirTabelaEmJanela()
        {
            // Calcula os totais das colunas R, S e T
            int totalR = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("R"));
            int totalS = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("S"));
            int totalT = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("T"));

            // Adiciona a linha de totais ao DataTable
            DataRow rowTotal = TabelaDeCircuitos.NewRow();
            rowTotal["Nome"] = "Totais";
            rowTotal["Circuito"] = DBNull.Value; // Deixa nulo para a coluna "Circuito"
            rowTotal["R"] = totalR;
            rowTotal["S"] = totalS;
            rowTotal["T"] = totalT;
            TabelaDeCircuitos.Rows.Add(rowTotal);

            // Configurações iniciais para a aplicação Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cria uma janela (Form) para exibir a tabela
            Form form = new Form
            {
                Text = $"Tabela do Quadro {Nome}", // Título da janela
                Width = 800, // Largura da janela
                Height = 600 // Altura da janela
            };

            // Cria um DataGridView para exibir os dados da tabela
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill, // Ajusta para preencher toda a janela
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Ajusta automaticamente o tamanho das colunas
                ReadOnly = true // Define como somente leitura
            };

            // Define o DataTable como fonte de dados do DataGridView
            dataGridView.DataSource = TabelaDeCircuitos;

            // Adiciona o DataGridView à janela
            form.Controls.Add(dataGridView);

            // Executa a aplicação Windows Forms, exibindo a janela criada
            Application.Run(form);
        }
    }
}
