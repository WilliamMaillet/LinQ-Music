using DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LinqExercicePresentationNet8
{
    public class Program
    {
        private static object? currentDataSource = null;
        private static string currentDataSourceName = string.Empty;
        private static IEnumerable<object>? currentResults = null;
        private static List<string> selectedFields = new List<string>();
        private static string currentTargetFormat = string.Empty;
        private static Dictionary<string, string> targetOptions = new Dictionary<string, string>();
        private static Dictionary<string, Func<object, object>> fieldGetters = new Dictionary<string, Func<object, object>>();

        static void Main(string[] args)
        {
            SetupTargetOptions();

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("======== CONVERTISSEUR DE DONNÉES MUSICALES ========");
                Console.ResetColor();
                Console.WriteLine("\nÉtat actuel:");
                Console.WriteLine($"  Source de données: {(string.IsNullOrEmpty(currentDataSourceName) ? "Non sélectionnée" : currentDataSourceName)}");
                Console.WriteLine($"  Format cible: {(string.IsNullOrEmpty(currentTargetFormat) ? "Non sélectionné" : currentTargetFormat)}");
                Console.WriteLine($"  Résultats: {(currentResults == null ? "Aucun" : "Disponibles")}");

                Console.WriteLine("\nMenu principal:");
                Console.WriteLine("1. Choisir une source de données");
                Console.WriteLine("2. Choisir un format cible");
                Console.WriteLine("3. Effectuer des opérations (recherche, tri, groupage)");
                Console.WriteLine("4. Afficher les résultats");
                Console.WriteLine("5. Exporter les résultats");
                Console.WriteLine("0. Quitter");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1": SelectDataSource(); break;
                    case "2": SelectTargetFormat(); break;
                    case "3": PerformOperations(); break;
                    case "4": DisplayResults(); break;
                    case "5": ExportResults(); break;
                    case "0": exit = true; break;
                    default: DisplayError("Option invalide"); break;
                }
            }
        }

        static void SetupTargetOptions()
        {
            targetOptions.Clear();
            targetOptions.Add("CSV", "Fichier CSV (valeurs séparées par des virgules)");
            targetOptions.Add("JSON", "Fichier JSON (JavaScript Object Notation)");
            targetOptions.Add("TXT", "Fichier texte simple (formaté)");
            targetOptions.Add("XML", "Fichier XML (à implémenter)");
            targetOptions.Add("HTML", "Page HTML (à implémenter)");
        }

        static void SelectTargetFormat()
        {
            Console.Clear();
            Console.WriteLine("=== SÉLECTION DU FORMAT CIBLE ===\n");
            Console.WriteLine("Vers quel format souhaitez-vous convertir vos données ?");

            int index = 1;
            foreach (var option in targetOptions)
            {
                Console.WriteLine($"{index++}. {option.Value}");
            }
            Console.WriteLine("0. Retour");

            Console.Write("\nVotre choix: ");
            string choice = Console.ReadLine() ?? string.Empty;

            if (choice == "0") return;

            if (int.TryParse(choice, out int selectedOption) &&
                selectedOption > 0 &&
                selectedOption <= targetOptions.Count)
            {
                var target = targetOptions.ElementAt(selectedOption - 1);
                currentTargetFormat = target.Key;

                Console.WriteLine($"\nFormat cible '{target.Value}' sélectionné.");
                WaitForKey();
            }
            else
            {
                DisplayError("Option invalide");
            }
        }

        static void SelectDataSource()
        {
            Console.Clear();
            Console.WriteLine("=== SÉLECTION DE LA SOURCE DE DONNÉES ===\n");
            Console.WriteLine("1. Albums");
            Console.WriteLine("2. Artistes (à implémenter)");
            Console.WriteLine("3. Genres (à implémenter)");
            Console.WriteLine("4. Chansons (à implémenter)");
            Console.WriteLine("0. Retour");

            Console.Write("\nVotre choix: ");
            string choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1":
                    currentDataSource = ListAlbumsData.ListAlbums;
                    currentDataSourceName = "Albums";
                    currentResults = null;
                    Console.WriteLine("Source de données 'Albums' sélectionnée.");
                    WaitForKey();
                    break;
                case "2":
                case "3":
                case "4":
                    DisplayError("Cette source de données n'est pas encore implémentée");
                    break;
                case "0":
                    return;
                default:
                    DisplayError("Option invalide");
                    break;
            }
        }

        static void PerformOperations()
        {
            if (currentDataSource == null)
            {
                DisplayError("Aucune source de données sélectionnée");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== OPÉRATIONS SUR LES DONNÉES ===\n");
            Console.WriteLine("1. Recherche");
            Console.WriteLine("2. Tri");
            Console.WriteLine("3. Groupage");
            Console.WriteLine("0. Retour");

            Console.Write("\nVotre choix: ");
            string choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1": PerformSearch(); break;
                case "2": PerformSort(); break;
                case "3": PerformGrouping(); break;
                case "0": return;
                default: DisplayError("Option invalide"); break;
            }
        }

        static void PerformSearch()
        {
            Console.Clear();
            Console.WriteLine("=== RECHERCHE DANS LES DONNÉES ===\n");

            if (currentDataSourceName == "Albums")
            {
                var albums = currentDataSource as List<Album> ?? new List<Album>();

                Console.WriteLine("Options de recherche:");
                Console.WriteLine("1. Recherche par titre");
                Console.WriteLine("2. Recherche par ID d'artiste");
                Console.WriteLine("0. Retour");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        Console.Write("Entrez un mot-clé à rechercher dans le titre: ");
                        string titleKeyword = Console.ReadLine() ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(titleKeyword))
                        {
                            DisplayError("Le mot-clé ne peut pas être vide.");
                            return;
                        }

                        var titleResults = albums
                            .Where(a => a.Title.Contains(titleKeyword, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (titleResults.Any())
                        {
                            Console.WriteLine($"\n{titleResults.Count} résultat(s) trouvé(s):");
                            
                            var formattedResults = titleResults
                                .Select((a, index) => $"Album n°{index + 1} : {a.Title} (ID: {a.AlbumId}, ArtistID: {a.ArtistId})")
                                .ToList();

                            foreach (var result in formattedResults)
                            {
                                Console.WriteLine(result);
                            }

                            currentResults = titleResults;
                            WaitForKey();
                        }
                        else
                        {
                            DisplayError("Aucun album trouvé avec ce mot-clé.");
                        }
                        break;

                    case "2":
                        Console.Write("Entrez l'ID de l'artiste: ");
                        if (int.TryParse(Console.ReadLine() ?? string.Empty, out int artistId))
                        {
                            var artistResults = albums
                                .Where(a => a.ArtistId == artistId)
                                .ToList();

                            if (artistResults.Any())
                            {
                                Console.WriteLine($"\n{artistResults.Count} résultat(s) trouvé(s):");
                                
                                var formattedResults = artistResults
                                    .Select((a, index) => $"Album n°{index + 1} : {a.Title} (ID: {a.AlbumId})")
                                    .ToList();

                                foreach (var result in formattedResults)
                                {
                                    Console.WriteLine(result);
                                }

                                currentResults = artistResults;
                                WaitForKey();
                            }
                            else
                            {
                                DisplayError("Aucun album trouvé pour cet artiste.");
                            }
                        }
                        else
                        {
                            DisplayError("ID d'artiste invalide. Veuillez entrer un nombre.");
                        }
                        break;

                    case "0": return;
                    default: DisplayError("Option invalide"); break;
                }
            }
            else
            {
                DisplayError("Recherche non implémentée pour cette source de données.");
            }
        }

        static void PerformSort()
        {
            Console.Clear();
            Console.WriteLine("=== TRI DES DONNÉES ===\n");

            if (currentDataSource == null)
            {
                DisplayError("Aucune source de données sélectionnée.");
                return;
            }

            if (currentDataSourceName == "Albums")
            {
                var albums = currentDataSource as List<Album> ?? new List<Album>();

                Console.WriteLine("Choisissez un critère de tri:");
                Console.WriteLine("1. Par titre (A-Z)");
                Console.WriteLine("2. Par titre (Z-A)");
                Console.WriteLine("3. Par ID d'album (croissant)");
                Console.WriteLine("4. Par ID d'album (décroissant)");
                Console.WriteLine("5. Par ID d'artiste (croissant)");
                Console.WriteLine("6. Par ID d'artiste (décroissant)");
                Console.WriteLine("0. Retour");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine() ?? string.Empty;

                IEnumerable<Album> sortedAlbums = null;
                string sortDescription = "";

                switch (choice)
                {
                    case "1":
                        sortedAlbums = albums.OrderBy(a => a.Title);
                        sortDescription = "Titre (A-Z)";
                        break;

                    case "2":
                        sortedAlbums = albums.OrderByDescending(a => a.Title);
                        sortDescription = "Titre (Z-A)";
                        break;

                    case "3":
                        sortedAlbums = albums.OrderBy(a => a.AlbumId);
                        sortDescription = "ID (croissant)";
                        break;

                    case "4":
                        sortedAlbums = albums.OrderByDescending(a => a.AlbumId);
                        sortDescription = "ID (décroissant)";
                        break;

                    case "5":
                        sortedAlbums = albums.OrderBy(a => a.ArtistId);
                        sortDescription = "ID d'artiste (croissant)";
                        break;

                    case "6":
                        sortedAlbums = albums.OrderByDescending(a => a.ArtistId);
                        sortDescription = "ID d'artiste (décroissant)";
                        break;

                    case "0": return;
                    default: DisplayError("Option invalide"); return;
                }

                if (sortedAlbums != null)
                {
                    Console.Clear();
                    Console.WriteLine($"=== ALBUMS TRIÉS PAR {sortDescription.ToUpper()} ===\n");

                    var sortedList = sortedAlbums.ToList();
                    
                    var formattedResults = sortedList
                        .Select((a, index) => $"Album n°{index + 1} : {a.Title} (ID: {a.AlbumId}, ArtistID: {a.ArtistId})")
                        .ToList();

                    foreach (var result in formattedResults)
                    {
                        Console.WriteLine(result);
                    }

                    currentResults = sortedList;
                    WaitForKey();
                }
            }
            else
            {
                DisplayError("Tri non implémenté pour cette source de données.");
            }
        }

        static void PerformGrouping()
        {
            Console.Clear();
            Console.WriteLine("=== GROUPAGE DES DONNÉES ===\n");

            if (currentDataSource == null)
            {
                DisplayError("Aucune source de données sélectionnée.");
                return;
            }

            if (currentDataSourceName == "Albums")
            {
                var albums = currentDataSource as List<Album> ?? new List<Album>();

                Console.WriteLine("Choisissez un critère de groupage:");
                Console.WriteLine("1. Par ID d'artiste");
                Console.WriteLine("2. Par première lettre du titre");
                Console.WriteLine("3. Statistiques par artiste");
                Console.WriteLine("0. Retour");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        var artistGroups = albums
                            .GroupBy(a => a.ArtistId)
                            .Select(g => new {
                                ArtistId = g.Key,
                                Albums = g.ToList(),
                                Count = g.Count()
                            })
                            .OrderBy(g => g.ArtistId)
                            .ToList();

                        Console.Clear();
                        Console.WriteLine("=== ALBUMS GROUPÉS PAR ID D'ARTISTE ===\n");

                        foreach (var group in artistGroups)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Artiste ID: {group.ArtistId} - {group.Count} album(s)");
                            Console.ResetColor();

                            var formattedAlbums = group.Albums
                                .Select((a, index) => $"  {index + 1}. {a.Title} (ID: {a.AlbumId})")
                                .ToList();

                            foreach (var album in formattedAlbums)
                            {
                                Console.WriteLine(album);
                            }

                            Console.WriteLine();
                        }

                        currentResults = artistGroups;
                        WaitForKey();
                        break;

                    case "2":
                        var letterGroups = albums
                            .GroupBy(a => a.Title.Substring(0, 1).ToUpper())
                            .Select(g => new {
                                FirstLetter = g.Key,
                                Albums = g.OrderBy(a => a.Title).ToList(),
                                Count = g.Count()
                            })
                            .OrderBy(g => g.FirstLetter)
                            .ToList();

                        Console.Clear();
                        Console.WriteLine("=== ALBUMS GROUPÉS PAR PREMIÈRE LETTRE DU TITRE ===\n");

                        foreach (var group in letterGroups)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Lettre: {group.FirstLetter} - {group.Count} album(s)");
                            Console.ResetColor();

                            var formattedAlbums = group.Albums
                                .Select((a, index) => $"  {index + 1}. {a.Title} (ID: {a.AlbumId}, ArtistID: {a.ArtistId})")
                                .ToList();

                            foreach (var album in formattedAlbums)
                            {
                                Console.WriteLine(album);
                            }

                            Console.WriteLine();
                        }

                        currentResults = letterGroups;
                        WaitForKey();
                        break;

                    case "3":
                        var artistStats = albums
                            .GroupBy(a => a.ArtistId)
                            .Select(g => new {
                                ArtistId = g.Key,
                                AlbumCount = g.Count(),
                                AvgTitleLength = g.Average(a => a.Title.Length),
                                MinId = g.Min(a => a.AlbumId),
                                MaxId = g.Max(a => a.AlbumId),
                                Titles = string.Join(", ", g.Select(a => a.Title))
                            })
                            .OrderByDescending(g => g.AlbumCount)
                            .ToList();

                        Console.Clear();
                        Console.WriteLine("=== STATISTIQUES PAR ARTISTE ===\n");

                        foreach (var stat in artistStats)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Artiste ID: {stat.ArtistId}");
                            Console.ResetColor();
                            Console.WriteLine($"  Nombre d'albums: {stat.AlbumCount}");
                            Console.WriteLine($"  Longueur moyenne des titres: {stat.AvgTitleLength:F1} caractères");
                            Console.WriteLine($"  Plage d'IDs: {stat.MinId} - {stat.MaxId}");
                            Console.WriteLine($"  Titres: {stat.Titles}");
                            Console.WriteLine();
                        }

                        currentResults = artistStats;
                        WaitForKey();
                        break;

                    case "0": return;
                    default: DisplayError("Option invalide"); break;
                }
            }
            else
            {
                DisplayError("Groupage non implémenté pour cette source de données.");
            }
        }

        static void DisplayResults()
        {
            if (currentResults == null)
            {
                DisplayError("Aucun résultat disponible. Effectuez d'abord des opérations.");
                return;
            }

            Type resultType = currentResults.GetType().GetGenericArguments()[0];

            Console.Clear();
            Console.WriteLine("=== AFFICHAGE DES RÉSULTATS ===\n");

            if (currentDataSourceName == "Albums" && resultType == typeof(Album))
            {
                var albums = currentResults as IEnumerable<Album> ?? Enumerable.Empty<Album>();
                DisplayAlbumList(albums.ToList());
            }
            else if (resultType.Name.Contains("AnonymousType"))
            {
                if (currentResults.Cast<object>().Any() &&
                    currentResults.Cast<object>().First().GetType().GetProperties().Any(p => p.Name == "Albums"))
                {
                    DisplayGroupedResults();
                }
                else if (currentResults.Cast<object>().Any() &&
                         currentResults.Cast<object>().First().GetType().GetProperties().Any(p => p.Name == "AlbumCount"))
                {
                    DisplayStatisticsResults();
                }
                else
                {
                    DisplayError("Type de résultat non reconnu");
                }
            }
            else
            {
                DisplayError("Affichage non implémenté pour ce type de résultat");
            }
        }

        static void DisplayAlbumList(List<Album> albums)
        {
            int currentPage = 1;
            int totalPages = (int)Math.Ceiling(albums.Count / 10.0);
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== AFFICHAGE DES ALBUMS ===\n");
                Console.WriteLine($"Page {currentPage}/{totalPages} - {albums.Count} album(s) au total\n");

                var pagedAlbums = albums
                    .Skip((currentPage - 1) * 10)
                    .Take(10)
                    .ToList();

                var formattedAlbums = pagedAlbums
                    .Select((a, index) => $"{(currentPage - 1) * 10 + index + 1}. {a.Title} (ID: {a.AlbumId}, Artiste: {a.ArtistId})")
                    .ToList();

                foreach (var album in formattedAlbums)
                {
                    Console.WriteLine(album);
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("  [P] Page précédente");
                Console.WriteLine("  [S] Page suivante");
                Console.WriteLine("  [F] Filtrer les résultats");
                Console.WriteLine("  [R] Retour au menu principal");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine()?.ToUpper() ?? string.Empty;

                switch (choice)
                {
                    case "P":
                        if (currentPage > 1) currentPage--;
                        break;
                    case "S":
                        if (currentPage < totalPages) currentPage++;
                        break;
                    case "F":
                        FilterAlbumResults(albums);
                        break;
                    case "R":
                        exit = true;
                        break;
                }
            }
        }

        static void FilterAlbumResults(List<Album> albums)
        {
            Console.Clear();
            Console.WriteLine("=== FILTRAGE DES RÉSULTATS ===\n");
            Console.WriteLine("1. Filtrer par mot-clé dans le titre");
            Console.WriteLine("2. Filtrer par ID d'artiste");
            Console.WriteLine("0. Retour");

            Console.Write("\nVotre choix: ");
            string choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1":
                    Console.Write("Entrez un mot-clé à rechercher dans le titre: ");
                    string keyword = Console.ReadLine() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        var filteredAlbums = albums
                            .Where(a => a.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (filteredAlbums.Any())
                        {
                            currentResults = filteredAlbums;
                            DisplayAlbumList(filteredAlbums);
                        }
                        else
                        {
                            DisplayError("Aucun album ne correspond à ce mot-clé");
                        }
                    }
                    break;

                case "2":
                    Console.Write("Entrez un ID d'artiste: ");
                    if (int.TryParse(Console.ReadLine() ?? string.Empty, out int artistId))
                    {
                        var filteredAlbums = albums
                            .Where(a => a.ArtistId == artistId)
                            .ToList();

                        if (filteredAlbums.Any())
                        {
                            currentResults = filteredAlbums;
                            DisplayAlbumList(filteredAlbums);
                        }
                        else
                        {
                            DisplayError("Aucun album ne correspond à cet ID d'artiste");
                        }
                    }
                    else
                    {
                        DisplayError("ID d'artiste invalide");
                    }
                    break;
            }
        }

        static void DisplayGroupedResults()
        {
            Console.Clear();
            Console.WriteLine("=== RÉSULTATS GROUPÉS ===\n");

            var groups = currentResults.Cast<dynamic>().ToList();

            int currentPage = 1;
            int totalPages = groups.Count;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== RÉSULTATS GROUPÉS ===\n");
                Console.WriteLine($"Groupe {currentPage}/{totalPages}\n");

                var group = groups[currentPage - 1];
                bool hasFirstLetter = HasProperty(group, "FirstLetter");

                Console.ForegroundColor = ConsoleColor.Yellow;
                if (hasFirstLetter)
                {
                    Console.WriteLine($"Groupe: Lettre '{group.FirstLetter}' - {group.Count} album(s)");
                }
                else
                {
                    Console.WriteLine($"Groupe: Artiste ID {group.ArtistId} - {group.Count} album(s)");
                }
                Console.ResetColor();

                var albums = ((IEnumerable<Album>)group.Albums).ToList();
                var formattedAlbums = albums
                    .Select((a, index) => $"  {index + 1}. {a.Title} (ID: {a.AlbumId}, ArtistID: {a.ArtistId})")
                    .ToList();

                foreach (var album in formattedAlbums)
                {
                    Console.WriteLine(album);
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("  [P] Groupe précédent");
                Console.WriteLine("  [S] Groupe suivant");
                Console.WriteLine("  [R] Retour au menu principal");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine()?.ToUpper() ?? string.Empty;

                switch (choice)
                {
                    case "P":
                        if (currentPage > 1) currentPage--;
                        break;
                    case "S":
                        if (currentPage < totalPages) currentPage++;
                        break;
                    case "R":
                        exit = true;
                        break;
                }
            }
        }

        static void DisplayStatisticsResults()
        {
            Console.Clear();
            Console.WriteLine("=== STATISTIQUES PAR ARTISTE ===\n");

            var stats = currentResults.Cast<dynamic>().ToList();

            int currentPage = 1;
            int totalPages = (int)Math.Ceiling(stats.Count / 5.0);
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== STATISTIQUES PAR ARTISTE ===\n");
                Console.WriteLine($"Page {currentPage}/{totalPages} - {stats.Count} artiste(s) au total\n");

                var pagedStats = stats
                    .Skip((currentPage - 1) * 5)
                    .Take(5)
                    .ToList();

                foreach (var stat in pagedStats)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Artiste ID: {stat.ArtistId}");
                    Console.ResetColor();
                    Console.WriteLine($"  Nombre d'albums: {stat.AlbumCount}");
                    Console.WriteLine($"  Longueur moyenne des titres: {stat.AvgTitleLength:F1} caractères");
                    Console.WriteLine($"  Plage d'IDs: {stat.MinId} - {stat.MaxId}");
                    string titles = stat.Titles.ToString();
                    if (titles.Length > 70)
                        titles = titles.Substring(0, 70) + "...";
                    Console.WriteLine($"  Titres: {titles}");
                    Console.WriteLine();
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("  [P] Page précédente");
                Console.WriteLine("  [S] Page suivante");
                Console.WriteLine("  [R] Retour au menu principal");

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine()?.ToUpper() ?? string.Empty;

                switch (choice)
                {
                    case "P":
                        if (currentPage > 1) currentPage--;
                        break;
                    case "S":
                        if (currentPage < totalPages) currentPage++;
                        break;
                    case "R":
                        exit = true;
                        break;
                }
            }
        }

        static bool HasProperty(dynamic obj, string propertyName)
        {
            return obj != null && obj.GetType().GetProperty(propertyName) != null;
        }

        static void ExportResults()
        {
            if (currentResults == null)
            {
                DisplayError("Aucun résultat disponible à exporter.");
                return;
            }

            if (string.IsNullOrEmpty(currentTargetFormat))
            {
                Console.Clear();
                Console.WriteLine("=== EXPORTATION DES RÉSULTATS ===\n");
                Console.WriteLine("Aucun format cible sélectionné.");
                Console.WriteLine("Voulez-vous choisir un format maintenant ? (O/N)");

                if ((Console.ReadLine() ?? string.Empty).Trim().ToUpper() == "O")
                {
                    SelectTargetFormat();
                    if (string.IsNullOrEmpty(currentTargetFormat)) return;
                }
                else
                {
                    return;
                }
            }

            Console.Clear();
            Console.WriteLine("=== EXPORTATION DES RÉSULTATS ===\n");
            Console.WriteLine($"Format cible: {currentTargetFormat}");

            Type resultType = currentResults.GetType().GetGenericArguments()[0];
            List<string> availableFields = new List<string>();

            if (currentDataSourceName == "Albums" && resultType == typeof(Album))
            {
                availableFields = new List<string> { "Id", "Title", "ArtistId" };

                fieldGetters = new Dictionary<string, Func<object, object>>
                {
                    { "Id", album => ((Album)album).AlbumId },
                    { "Title", album => ((Album)album).Title },
                    { "ArtistId", album => ((Album)album).ArtistId }
                };
            }
            else if (resultType.Name.Contains("AnonymousType"))
            {
                var properties = resultType.GetProperties();
                foreach (var prop in properties)
                {
                    availableFields.Add(prop.Name);
                    fieldGetters.Add(prop.Name, obj => prop.GetValue(obj));
                }
            }

            selectedFields.Clear();

            Console.Clear();
            Console.WriteLine("=== SÉLECTION DES CHAMPS À EXPORTER ===\n");
            Console.WriteLine("Champs disponibles:");

            for (int i = 0; i < availableFields.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableFields[i]}");
            }

            Console.WriteLine("\nSélectionnez les champs à exporter (séparés par des virgules)");
            Console.WriteLine("Exemple: 1,3 pour exporter le premier et troisième champ");
            Console.WriteLine("Entrez 'A' pour sélectionner tous les champs");

            Console.Write("\nVotre sélection: ");
            string selection = (Console.ReadLine() ?? string.Empty).ToUpper();

            if (selection == "A")
            {
                selectedFields.AddRange(availableFields);
            }
            else
            {
                var selections = selection.Split(',');
                foreach (var sel in selections)
                {
                    if (int.TryParse(sel.Trim(), out int index) && index >= 1 && index <= availableFields.Count)
                    {
                        selectedFields.Add(availableFields[index - 1]);
                    }
                }
            }

            if (selectedFields.Count == 0)
            {
                DisplayError("Aucun champ valide sélectionné pour l'export");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== ENREGISTREMENT DU FICHIER ===\n");
            Console.Write("Entrez un nom pour le fichier d'export (sans extension): ");
            string fileName = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            try
            {
                string extension = "";
                string content = "";

                switch (currentTargetFormat)
                {
                    case "CSV": extension = ".csv"; content = ExportToCsv(); break;
                    case "JSON": extension = ".json"; content = ExportToJson(); break;
                    case "TXT": extension = ".txt"; content = ExportToText(); break;
                    case "XML": extension = ".xml"; content = ExportToXml(); break;
                    case "HTML": extension = ".html"; content = ExportToHtml(); break;
                    default: DisplayError("Format non supporté"); return;
                }

                string fullPath = Path.Combine(Environment.CurrentDirectory, fileName + extension);
                File.WriteAllText(fullPath, content);

                DisplayExportPreview(content, fullPath);
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'export: {ex.Message}");
            }
        }

        static void DisplayExportPreview(string content, string fullPath)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nExport réussi!\nFichier enregistré: {fullPath}");
            Console.ResetColor();

            Console.WriteLine("\nAperçu du contenu exporté:");
            Console.WriteLine(new string('-', 40));

            var lines = content.Split('\n');
            for (int i = 0; i < Math.Min(10, lines.Length); i++)
            {
                if (lines[i].Length > 100)
                    Console.WriteLine(lines[i].Substring(0, 97) + "...");
                else
                    Console.WriteLine(lines[i]);
            }

            if (lines.Length > 10)
            {
                Console.WriteLine("...");
                Console.WriteLine($"(Total: {lines.Length} lignes)");
            }

            Console.WriteLine(new string('-', 40));
            WaitForKey();
        }

        static string ExportToCsv()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine(string.Join(",", selectedFields));

            if (currentDataSourceName == "Albums" && currentResults.GetType().GetGenericArguments()[0] == typeof(Album))
            {
                var albums = currentResults.Cast<Album>();
                
                var csvLines = albums.Select(album =>
                    string.Join(",", selectedFields.Select(field =>
                        fieldGetters[field](album)?.ToString().Replace(",", ";") ?? ""))
                );

                foreach (var line in csvLines)
                {
                    result.AppendLine(line);
                }
            }
            else if (currentResults.GetType().GetGenericArguments()[0].Name.Contains("AnonymousType"))
            {
                var items = currentResults.Cast<object>();

                foreach (var item in items)
                {
                    var values = selectedFields.Select(field =>
                        fieldGetters[field](item)?.ToString().Replace(",", ";") ?? "");
                    result.AppendLine(string.Join(",", values));
                }
            }

            return result.ToString();
        }

        static string ExportToJson()
        {
            var jsonItems = new List<Dictionary<string, object>>();

            if (currentDataSourceName == "Albums" && currentResults.GetType().GetGenericArguments()[0] == typeof(Album))
            {
                var albums = currentResults.Cast<Album>();
                
                jsonItems = albums.Select(album =>
                    selectedFields.ToDictionary(
                        field => field,
                        field => fieldGetters[field](album))
                ).ToList();
            }
            else if (currentResults.GetType().GetGenericArguments()[0].Name.Contains("AnonymousType"))
            {
                var items = currentResults.Cast<object>();

                foreach (var item in items)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var field in selectedFields)
                    {
                        dict[field] = fieldGetters[field](item);
                    }
                    jsonItems.Add(dict);
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(jsonItems, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        static string ExportToText()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine(string.Join("\t", selectedFields));
            result.AppendLine(new string('-', 50));

            if (currentDataSourceName == "Albums" && currentResults.GetType().GetGenericArguments()[0] == typeof(Album))
            {
                var albums = currentResults.Cast<Album>();

                int index = 1;
                foreach (var album in albums)
                {
                    result.AppendLine($"Élément #{index++}");
                    foreach (var field in selectedFields)
                    {
                        result.AppendLine($"  {field}: {fieldGetters[field](album)}");
                    }
                    result.AppendLine();
                }
            }
            else if (currentResults.GetType().GetGenericArguments()[0].Name.Contains("AnonymousType"))
            {
                var items = currentResults.Cast<object>();

                int index = 1;
                foreach (var item in items)
                {
                    result.AppendLine($"Élément #{index++}");
                    foreach (var field in selectedFields)
                    {
                        result.AppendLine($"  {field}: {fieldGetters[field](item)}");
                    }
                    result.AppendLine();
                }
            }

            return result.ToString();
        }

        static string ExportToXml()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            result.AppendLine("<Results>");

            Type resultType = currentResults.GetType().GetGenericArguments()[0];

            if (currentDataSourceName == "Albums" && resultType == typeof(Album))
            {
                var albums = currentResults.Cast<Album>();

                foreach (var album in albums)
                {
                    result.AppendLine("  <Album>");
                    foreach (var field in selectedFields)
                    {
                        var value = fieldGetters[field](album)?.ToString() ?? "";
                        value = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        result.AppendLine($"    <{field}>{value}</{field}>");
                    }
                    result.AppendLine("  </Album>");
                }
            }
            else if (resultType.Name.Contains("AnonymousType"))
            {
                var items = currentResults.Cast<object>();

                foreach (var item in items)
                {
                    result.AppendLine("  <Item>");
                    foreach (var field in selectedFields)
                    {
                        var value = fieldGetters[field](item)?.ToString() ?? "";
                        value = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        result.AppendLine($"    <{field}>{value}</{field}>");
                    }
                    result.AppendLine("  </Item>");
                }
            }

            result.AppendLine("</Results>");
            return result.ToString();
        }

        static string ExportToHtml()
        {
            var result = new System.Text.StringBuilder();
            
            result.AppendLine("<!DOCTYPE html>");
            result.AppendLine("<html lang=\"fr\">");
            result.AppendLine("<head>");
            result.AppendLine("  <meta charset=\"UTF-8\">");
            result.AppendLine("  <title>Résultats exportés</title>");
            result.AppendLine("  <style>");
            result.AppendLine("    table { border-collapse: collapse; width: 100%; }");
            result.AppendLine("    th, td { text-align: left; padding: 8px; }");
            result.AppendLine("    th { background-color: #4CAF50; color: white; }");
            result.AppendLine("    tr:nth-child(even) { background-color: #f2f2f2; }");
            result.AppendLine("  </style>");
            result.AppendLine("</head>");
            result.AppendLine("<body>");
            result.AppendLine("  <h1>Résultats exportés</h1>");
            
            result.AppendLine("  <table>");
            
            result.AppendLine("    <tr>");
            foreach (var field in selectedFields)
            {
                result.AppendLine($"      <th>{field}</th>");
            }
            result.AppendLine("    </tr>");
            
            Type resultType = currentResults.GetType().GetGenericArguments()[0];
            
            if (currentDataSourceName == "Albums" && resultType == typeof(Album))
            {
                var albums = currentResults.Cast<Album>();
                
                foreach (var album in albums)
                {
                    result.AppendLine("    <tr>");
                    foreach (var field in selectedFields)
                    {
                        var value = fieldGetters[field](album)?.ToString() ?? "";
                        value = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        result.AppendLine($"      <td>{value}</td>");
                    }
                    result.AppendLine("    </tr>");
                }
            }
            else if (resultType.Name.Contains("AnonymousType"))
            {
                var items = currentResults.Cast<object>();
                
                foreach (var item in items)
                {
                    result.AppendLine("    <tr>");
                    foreach (var field in selectedFields)
                    {
                        var value = fieldGetters[field](item)?.ToString() ?? "";
                        value = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        result.AppendLine($"      <td>{value}</td>");
                    }
                    result.AppendLine("    </tr>");
                }
            }
            
            result.AppendLine("  </table>");
            result.AppendLine("  <p>Exporté le " + DateTime.Now.ToString() + "</p>");
            result.AppendLine("</body>");
            result.AppendLine("</html>");
            
            return result.ToString();
        }

        static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nErreur: {message}");
            Console.ResetColor();
            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey(true);
        }
    }
}