using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public class Storage : BaseStorage
    {
        // folders
        // private const string UsersFolderName = "users";

        // files
        // private const string CollageFileName = "collage";

        //#region Shows

        //private static async Task StoreShowAsync(TraktShow traktShow)
        //{
        //    // store show
        //    var showId = traktShow.Ids.Trakt.ToString();
        //    await SaveAsync(traktShow, await GetFolderAsync(ShowsFolderName), showId);

        //    // store show ID to show list
        //    await ShowListSemaphore.WaitAsync();
        //    var showList = await LoadShowListAsync();
        //    showList.Add(showId);
        //    await StoreShowListAsync(showList);
        //    ShowListSemaphore.Release();
        //}

        //private static async Task StoreShowsAsync(IEnumerable<TraktShow> traktShows)
        //{
        //    var folder = await GetFolderAsync(ShowsFolderName);
        //    await ShowListSemaphore.WaitAsync();
        //    var showList = await LoadShowListAsync();

        //    foreach (var show in traktShows)
        //    {
        //        // store show
        //        var showId = show.Ids.Trakt.ToString();
        //        await SaveAsync(show, folder, showId);

        //        // store show ID to show list
        //        showList.Add(showId);
        //    }
        //    await StoreShowListAsync(showList);
        //    ShowListSemaphore.Release();
        //}

        //private static async Task<TraktShow> LoadShowAsync(string showId)
        //{
        //    return await LoadAsync<TraktShow>(await GetFolderAsync(ShowsFolderName), showId);
        //}

        ///*private static async Task<HashSet<string>> GetShowListAsync()
        //{
        //    if (_showList == null)
        //    {
        //        // only first task needs to load list
        //        await ShowListSemaphore.WaitAsync();
        //        if (_showList == null)
        //        {
        //            _showList = await LoadShowListAsync();
        //        }
        //        ShowListSemaphore.Release();
        //    }
        //    return _showList;
        //}*/

        //private static async Task StoreShowListAsync(HashSet<string> showList)
        //{
        //    await SaveAsync(showList, await GetFolderAsync(ShowsFolderName), ShowListFileName);
        //}

        //private static async Task<HashSet<string>> LoadShowListAsync()
        //{
        //    return await LoadAsync<HashSet<string>>(await GetFolderAsync(ShowsFolderName), ShowListFileName) ?? new HashSet<string>();
        //}

        //#endregion
    }
}
