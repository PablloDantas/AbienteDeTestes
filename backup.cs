// namespace AmbienteDeTestes;
//
// public class backup
// {
//     // Circuitos pré-definidos
//     List<int> chu1 = new List<int> { 2500, 2500 }; // Circuito bifásico
//     List<int> chu2 = new List<int> { 3000, 3000 }; // Circuito bifásico
//     List<int> tue = new List<int> { 3000 }; // Circuito monofásico
//     List<int> tug = new List<int> { 5000 }; // Circuito monofásico
//     List<int> ilu = new List<int> { 2000 }; // Circuito monofásico
//     List<int> mb = new List<int> { 2000, 2000, 2000 }; // Circuito trifásico
//
//     // Lista de circuitos que será processada
//     List<List<int>> qd1 = new List<List<int>> { chu1, chu2, tue, tug, ilu, mb };
//
//     // Ordena os circuitos por:
//     // 1. Número de fases (maior para menor)
//     // 2. Soma total da carga (maior para menor)
//     var listaOrdenada = qd1.OrderByDescending(x => x.Count)
//         .ThenByDescending(x => x.Sum())
//         .ToList();
//
//     // Listas que representam as fases R, S e T
//     List<int> r = new List<int>();
//     List<int> s = new List<int>();
//     List<int> t = new List<int>();
//
//     // Agrupamento das fases
//     List<List<int>> fases = new List<List<int>> { r, s, t };
//
//         // Processa cada circuito da lista ordenada
//         for (int i = 0; i<listaOrdenada.Count;
//     i++)
//     {
//         List<int> circuito = listaOrdenada[i]; // Circuito atual
//
//         // Distribui circuitos trifásicos
//         if (circuito.Count == 3)
//         {
//             r.Add(circuito[0]); // Adiciona a carga na fase R
//             s.Add(circuito[1]); // Adiciona a carga na fase S
//             t.Add(circuito[2]); // Adiciona a carga na fase T
//         }
//         // Distribui circuitos bifásicos
//         else if (circuito.Count == 2)
//         {
//             var fasesUtilizadas = new List<List<int>>(); // Controle das fases já usadas para este circuito
//
//             for (int j = 0; j < circuito.Count; j++)
//             {
//                 int carga = circuito[j]; // Carga atual do circuito
//
//                 // Seleciona a fase com a menor soma de carga que ainda não foi usada
//                 var faseMenorCarga = fases
//                     .Where(lista => !fasesUtilizadas.Contains(lista))
//                     .OrderByDescending(lista => lista.Sum())
//                     .Last();
//
//                 faseMenorCarga.Add(carga); // Adiciona a carga à fase selecionada
//                 fasesUtilizadas.Add(faseMenorCarga); // Marca a fase como utilizada
//             }
//         }
//         // Distribui circuitos monofásicos
//         else if (circuito.Count == 1)
//         {
//             int carga = circuito[0]; // Carga do circuito monofásico
//
//             // Seleciona a fase com a menor soma de carga
//             var faseMenorCarga = fases.OrderByDescending(lista => lista.Sum()).Last();
//
//             faseMenorCarga.Add(carga); // Adiciona a carga à fase selecionada
//         }
//     }
// }
