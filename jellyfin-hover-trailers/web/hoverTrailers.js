(function () {
    const pluginId = "FCBEE76B-7170-410F-9029-B3B2E5F5EDA4";
    let tmdbKey = "";

    function init() {
        ApiClient.getPluginConfiguration(pluginId).then(function (config) {
            tmdbKey = config.TmdbApiKey;
            if (tmdbKey) {
                console.log("Hover Trailers: Initialized with API Key.");
                setupEventListeners();
            }
        });
    }

    function setupEventListeners() {
        document.addEventListener('mouseenter', function (e) {
            const card = e.target.closest('.card');
            if (card && !card.querySelector('.hover-trailer-active')) {
                const itemId = card.getAttribute('data-id');
                playTrailer(itemId, card);
            }
        }, true);
    }

    async function playTrailer(itemId, card) {
        const item = await ApiClient.getItem(ApiClient.getCurrentUserId(), itemId);
        const tmdbId = item.ProviderIds ? item.ProviderIds.Tmdb : null;
        if (!tmdbId) return;

        fetch(`https://api.themoviedb.org/3/movie/${tmdbId}/videos?api_key=${tmdbKey}`)
            .then(res => res.json())
            .then(data => {
                const trailer = data.results.find(v => v.type === 'Trailer' && v.site === 'YouTube');
                if (trailer) {
                    console.log("Playing trailer: " + trailer.key);
                    // Add player logic here
                }
            });
    }

    if (window.ApiClient) {
        init();
    }
})();