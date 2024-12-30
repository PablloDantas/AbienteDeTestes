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

            // Adiciona os circuitos a uma lista
            List<Circuito> circuitos = new List<Circuito> { chu1, chu2, tue, tug, ilu, mb };

            // Cria um quadro e distribui as fases
            Quadro qd1 = new Quadro("QD1", circuitos);

            qd1.BalancearCargas();

            // Exibe a tabela no console
            // qd1.ExibirTabela();
            qd1.ExibirTabelaEmJanela();
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

        private List<int> _listaDeCargas;

        /// <summary>
        /// Obtém a lista de cargas distribuídas entre as fases do circuito.
        /// </summary>
        public List<int> ListaDeCargas
        {
            get
            {
                if (_listaDeCargas == null)
                {
                    _listaDeCargas = new List<int>();

                    // Calcula a carga por fase
                    int cargaPorFase = CargaTotal / QuantidadeDeFases;

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
        /// </summary>
        /// <param name="nome">Nome do quadro.</param>
        /// <param name="circuitos">Lista de circuitos a serem distribuídos no quadro.</param>
        public Quadro(string nome, List<Circuito> circuitos)
        {
            Nome = nome;
            Circuitos = circuitos;
            TabelaDeCircuitos = new DataTable
            {
                Columns =
                {
                    new DataColumn("Nome", typeof(string)),
                    new DataColumn("Circuito", typeof(Circuito)) { AllowDBNull = true },
                    new DataColumn("R", typeof(int)),
                    new DataColumn("S", typeof(int)),
                    new DataColumn("T", typeof(int)),
                }
            };
        }

        public void BalancearCargas()
        {
            DataTable tabelaCircuitos = this.DistribuirCircuitos(this.Circuitos);
            DataTable tabelaCircuitosDecrescente = this.DistribuirCircuitosDecrescente(
                this.Circuitos
            );
            DataTable tabelaCircuitosCrescente = this.DistribuirCircuitosCrescente(this.Circuitos);
            DataTable tabelaCircuitosAleatorios = this.DistribuirCircuitosAleatoriamente(
                this.Circuitos
            );

            int amplitudeCircuitos = this.DiferençaDeCarga(tabelaCircuitos);
            int amplitudeCircuitosDecrescente = this.DiferençaDeCarga(tabelaCircuitosDecrescente);
            int amplitudeCircuitosCrescente = this.DiferençaDeCarga(tabelaCircuitosCrescente);
            int amplitudeCircuitosAleatorios = this.DiferençaDeCarga(tabelaCircuitosAleatorios);

            List<DataTable> tabelas = new List<DataTable>
            {
                tabelaCircuitos,
                tabelaCircuitosDecrescente,
                tabelaCircuitosCrescente,
                tabelaCircuitosAleatorios
            };

            List<int> amplitudes = new List<int>
            {
                amplitudeCircuitos,
                amplitudeCircuitosDecrescente,
                amplitudeCircuitosCrescente,
                amplitudeCircuitosAleatorios
            };

            int menorAmplitude = amplitudes.IndexOf(amplitudes.Min());

            DataTable tabelaBalanceada = tabelas[menorAmplitude];

            this.TabelaDeCircuitos = tabelaBalanceada;
        }

        /// <summary>
        /// Distribui os circuitos elétricos entre as fases R, S e T sem ordenação específica.
        /// </summary>
        public DataTable DistribuirCircuitos(List<Circuito> circuitos)
        {
            // Inicializa listas auxiliares: Cargas, Circuitos e nomes de circuitos
            List<int> faseR = new List<int>();
            List<int> faseS = new List<int>();
            List<int> faseT = new List<int>();
            List<Circuito> circuitosDistribuidos = new List<Circuito>();
            List<string> nomesCircuitos = new List<string>();

            // Agrupamentos das fases
            List<List<int>> fases = new List<List<int>> { faseR, faseS, faseT };

            foreach (var circuito in circuitos)
            {
                List<int> listaDeCargasCircuito = circuito.ListaDeCargas;

                // Distribui circuitos trifásicos
                if (listaDeCargasCircuito.Count == 3)
                {
                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                    faseR.Add(listaDeCargasCircuito[0]); // Adiciona a carga na fase R
                    faseS.Add(listaDeCargasCircuito[1]); // Adiciona a carga na fase S
                    faseT.Add(listaDeCargasCircuito[2]); // Adiciona a carga na fase T
                }
                // Distribui circuitos bifásicos
                else if (listaDeCargasCircuito.Count == 2)
                {
                    var fasesUtilizadas = new List<List<int>>(); // Controle das fases já usadas para este circuito

                    for (int j = 0; j < listaDeCargasCircuito.Count; j++)
                    {
                        int carga = listaDeCargasCircuito[j]; // Carga atual do circuito

                        // Seleciona a fase com a menor soma de carga que ainda não foi usada
                        var faseMenorCarga = fases
                            .Where(lista => !fasesUtilizadas.Contains(lista))
                            .MinBy(lista => lista.Sum());

                        faseMenorCarga.Add(carga); // Adiciona a carga à fase selecionada
                        fasesUtilizadas.Add(faseMenorCarga); // Marca a fase como utilizada
                    }

                    // Preenche a fase não utilizada com carga zero
                    var faseNaoUtilizada = fases.First(lista => !fasesUtilizadas.Contains(lista));
                    faseNaoUtilizada.Add(0);

                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
                // Distribui circuitos monofásicos
                else if (listaDeCargasCircuito.Count == 1)
                {
                    int carga = listaDeCargasCircuito[0]; // Carga do circuito monofásico

                    // Seleciona a fase com a menor soma de carga
                    var faseMenorCarga = fases.MinBy(lista => lista.Sum());

                    faseMenorCarga.Add(carga); // Adiciona a carga à fase selecionada

                    // Preenche as outras fases com carga zero
                    var fasesNaoUtilizadas = fases.Where(lista => lista != faseMenorCarga).ToList();

                    foreach (var fase in fasesNaoUtilizadas)
                        fase.Add(0);

                    circuitosDistribuidos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
            }

            return GerarTabela(nomesCircuitos, circuitosDistribuidos, faseR, faseS, faseT);
        }

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T, ordenando-os por número de fases e carga total de forma decrescente.
        /// </summary>
        public DataTable DistribuirCircuitosDecrescente(List<Circuito> circuitos)
        {
            // Ordena os circuitos por número de fases e carga total
            var circuitosOrdenados = circuitos
                .OrderByDescending(x => x.ListaDeCargas.Count)
                .ThenByDescending(x => x.ListaDeCargas.Sum())
                .ToList();

            return DistribuirCircuitos(circuitosOrdenados);
        }

        public DataTable DistribuirCircuitosCrescente(List<Circuito> circuitos)
        {
            // Ordena os circuitos por número de fases e carga total
            var circuitosOrdenados = circuitos
                .OrderBy(x => x.ListaDeCargas.Count)
                .ThenBy(x => x.ListaDeCargas.Sum())
                .ToList();

            return DistribuirCircuitos(circuitosOrdenados);
        }

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T de forma aleatória, buscando equilibrar as cargas.
        /// </summary>
        public DataTable DistribuirCircuitosAleatoriamente(List<Circuito> circuitos)
        {
            Random random = new Random();

            // Ordena os circuitos de forma aleatória e por carga total decrescente
            var circuitosOrdenados = circuitos
                .OrderBy(x => random.Next())
                .ThenByDescending(x => x.ListaDeCargas.Sum())
                .ToList();

            return DistribuirCircuitos(circuitosOrdenados);
        }

        public List<int> CalcularCargaTotal(DataTable tabelaDeCargas)
        {
            // Calcula os totais das colunas R, S e T
            int totalR = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("R"));
            int totalS = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("S"));
            int totalT = tabelaDeCargas.AsEnumerable().Sum(row => row.Field<int>("T"));

            List<int> fasesSomadas = new List<int> { totalR, totalS, totalT };

            return fasesSomadas;
        }

        public int DiferençaDeCarga(DataTable tabelaDeCargas)
        {
            List<int> cargasTotais = CalcularCargaTotal(tabelaDeCargas);

            int maior = cargasTotais.Max();
            int menor = cargasTotais.Min();

            return maior - menor;
        }

        /// <summary>
        /// Exibe a tabela de distribuição de circuitos no console.
        /// </summary>
        public DataTable GerarTabela(
            List<string> nomesDosCircuitos,
            List<Circuito> circuitos,
            List<int> faseR,
            List<int> faseS,
            List<int> faseT
        )
        {
            DataTable tabelaDeCircuitos = new DataTable
            {
                Columns =
                {
                    new DataColumn("Nome", typeof(string)),
                    new DataColumn("Circuito", typeof(Circuito)) { AllowDBNull = true },
                    new DataColumn("R", typeof(int)),
                    new DataColumn("S", typeof(int)),
                    new DataColumn("T", typeof(int)),
                }
            };
            for (int i = 0; i < circuitos.Count; i++)
            {
                tabelaDeCircuitos.Rows.Add(
                    nomesDosCircuitos[i],
                    circuitos[i],
                    faseR[i],
                    faseS[i],
                    faseT[i]
                );
            }

            return tabelaDeCircuitos;
        }

        public void ExibirTabela()
        {
            var consoleTable = new ConsoleTable();

            // Adiciona os nomes das colunas
            foreach (DataColumn coluna in TabelaDeCircuitos.Columns)
            {
                consoleTable.AddColumn(new[] { coluna.ColumnName });
            }

            // Adiciona as linhas da tabela
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

            // Adiciona a linha de totais
            consoleTable.AddRow("Totais", "", totalR, totalS, totalT);

            // Imprime no console
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

            // Cria a aplicação Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cria uma janela (Form)
            Form form = new Form
            {
                Text = $"Tabela do Quadro {Nome}",
                Width = 800,
                Height = 600
            };

            // Cria um DataGridView para exibir a tabela
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true // Define como somente leitura
            };

            // Define o DataTable como fonte de dados do DataGridView
            dataGridView.DataSource = TabelaDeCircuitos;

            // Adiciona o DataGridView à janela
            form.Controls.Add(dataGridView);

            // Executa o aplicativo Windows Forms
            Application.Run(form);
        }
    }
}
