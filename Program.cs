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
        static void Main(string[] args)
        {
            // Inicializa circuitos com suas cargas e número de fases
            Circuito chu1 = new Circuito("Chuveiro 01", 6000, 2);
            Circuito chu2 = new Circuito("Chuveiro 02", 5000, 2);
            Circuito tug = new Circuito("Tomada de uso geral", 5000, 1);
            Circuito tue = new Circuito("Tomada de uso específico", 3000, 1);
            Circuito ilu = new Circuito("Iluminação", 2000, 1);
            Circuito mb = new Circuito("Motobomba", 6000, 3);

            // Adiciona os circuitos a uma lista
            List<Circuito> circuitos = new List<Circuito> { chu1, chu2, tue, tug, ilu, mb };

            // Cria um quadro e distribui as fases
            Quadro qd1 = new Quadro("QD1", circuitos);
            qd1.DistribuirCircuitos();

            // Exibe a tabela
            qd1.ExibirTabela();
            qd1.ExibirTabelaEmJanela();
        }
    }

    /// <summary>
    /// Representa um circuito elétrico.
    /// </summary>
    public class Circuito
    {
        public string Nome { get; set; }
        public int CargaTotal { get; set; }
        public int QuantidadeDeFases { get; set; }

        private List<int>? _listaDeCargas;

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

                    int cargaPorFase = CargaTotal / QuantidadeDeFases;

                    for (int i = 0; i < QuantidadeDeFases; i++)
                        _listaDeCargas.Add(cargaPorFase);
                }

                return _listaDeCargas;
            }
        }

        /// <summary>
        /// Construtor do circuito.
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
    /// Representa um quadro elétrico.
    /// </summary>
    public class Quadro
    {
        public string Nome { get; set; }
        public List<Circuito> Circuitos { get; set; }
        public DataTable TabelaDeCircuitos { get; set; }

        /// <summary>
        /// Construtor do quadro.
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

        /// <summary>
        /// Distribui os circuitos entre as fases R, S e T, buscando equilibrar as cargas.
        /// </summary>
        public void DistribuirCircuitos()
        {
            // Ordena os circuitos por número de fases e carga total
            var listaDeCircuitosOrdenados = Circuitos
                .OrderByDescending(x => x.ListaDeCargas.Count)
                .ThenByDescending(x => x.ListaDeCargas.Sum())
                .ToList();

            // Inicializa listas para representar as fases
            List<int> faseR = new List<int>();
            List<int> faseS = new List<int>();
            List<int> faseT = new List<int>();
            List<Circuito> circuitos = new List<Circuito>();
            List<string> nomesCircuitos = new List<string>();

            // Agrupamentos das fases
            List<List<int>> fases = new List<List<int>> { faseR, faseS, faseT };

            for (int i = 0; i < listaDeCircuitosOrdenados.Count; i++)
            {
                Circuito circuito = listaDeCircuitosOrdenados[i]; // Circuito atual
                List<int> listaDeCargasCircuito = circuito.ListaDeCargas;

                // Distribui circuitos trifásicos
                if (listaDeCargasCircuito.Count == 3)
                {
                    circuitos.Add(circuito);
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

                    var faseNãoUtilizada = fases.First(lista => !fasesUtilizadas.Contains(lista));

                    faseNãoUtilizada.Add(0);

                    circuitos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
                // Distribui circuitos monofásicos
                else if (listaDeCargasCircuito.Count == 1)
                {
                    int carga = listaDeCargasCircuito[0]; // Carga do circuito monofásico

                    // Seleciona a fase com a menor soma de carga
                    var faseMenorCarga = fases.MinBy(lista => lista.Sum());

                    faseMenorCarga.Add(carga); // Adiciona a carga à fase selecionada

                    var fasesNãoUtilizadas = fases
                        .Where(lista => lista != faseMenorCarga)
                        .Select(lista => lista);

                    foreach (var fase in fasesNãoUtilizadas)
                        fase.Add(0);

                    circuitos.Add(circuito);
                    nomesCircuitos.Add(circuito.Nome);
                }
            }

            // Popula a tabela
            for (int i = 0; i < circuitos.Count; i++)
            {
                TabelaDeCircuitos.Rows.Add(
                    nomesCircuitos[i],
                    circuitos[i],
                    faseR[i],
                    faseS[i],
                    faseT[i]
                );
            }
        }

        /// <summary>
        /// Exibe a tabela no console.
        /// </summary>
        public void ExibirTabela()
        {
            var consoleTable = new ConsoleTable();

            // Adiciona os nomes das colunas
            foreach (DataColumn coluna in TabelaDeCircuitos.Columns)
            {
                consoleTable.AddColumn(new[] { coluna.ColumnName });
            }

            // Adiciona as linhas
            foreach (DataRow row in TabelaDeCircuitos.Rows)
            {
                consoleTable.AddRow(row.ItemArray);
            }

            // Calcula os totais das colunas R, S e T
            int totalR = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("R"));
            int totalS = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("S"));
            int totalT = TabelaDeCircuitos.AsEnumerable().Sum(row => row.Field<int>("T"));

            // Adiciona a linha de totais
            consoleTable.AddRow("Totais", null, totalR, totalS, totalT);

            // Imprime no console
            consoleTable.Write(Format.Alternative);
        }

        /// <summary>
        /// Exibe a tabela em uma janela usando Windows Forms.
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
            rowTotal["Circuito"] = DBNull.Value; // Deixe nulo para a coluna "Circuito"
            rowTotal["R"] = totalR;
            rowTotal["S"] = totalS;
            rowTotal["T"] = totalT;
            TabelaDeCircuitos.Rows.Add(rowTotal);

            // Cria a aplicação Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cria uma janela
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
