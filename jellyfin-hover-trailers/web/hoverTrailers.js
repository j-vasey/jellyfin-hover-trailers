(function () {
    let hoverTimeout;
    let activeIframe = null;

    document.addEventListener('mouseover', async function (e) {
        const card = e.target.closest('.card');
        if (!card || activeIframe || card.getAttribute('data-type') !== 'Movie') return;

        hoverTimeout = setTimeout(async () => {
            const itemId = card.getAttribute('data-id');
            const container = card.querySelector('.cardImageContainer');

            // 1. Get Item Details from Jellyfin API
            const item = await ApiClient.getItem(ApiClient.getCurrentUserId(), itemId);
            const tmdbId = item.ProviderIds ? item.ProviderIds.Tmdb : null;

            if (tmdbId) {
                // 2. Fetch Trailer Key from TMDb
                const youtubeKey = await fetchTmdbTrailer(tmdbId);
                if (youtubeKey) {
                    playTrailer(container, youtubeKey);
                }
            }
        }, 800);
    });

    async function fetchTmdbTrailer(tmdbId) {
        // Fetch your API Key from the Plugin Configuration
        const config = await ApiClient.getPluginConfiguration("d4f5e6b7-c8a9-4d0e-9f1a-2b3c4d5e6f70");
        const apiKey = config.TmdbApiKey;

        if (!apiKey) return null;

        try {
            const response = await fetch(`https://api.themoviedb.org/3/movie/${tmdbId}/videos?api_key=${apiKey}`);
            const data = await response.json();
            // Find the first official YouTube trailer
            const trailer = data.results.find(v => v.site === 'YouTube' && v.type === 'Trailer' && v.official);
            return trailer ? trailer.key : null;
        } catch (e) {
            console.error("Error fetching TMDb trailer:", e);
            return null;
        }
    }

    function playTrailer(container, key) {
        activeIframe = document.createElement('iframe');
        activeIframe.src = `https://www.youtube.com/embed/${key}?autoplay=1&mute=1&controls=0&modestbranding=1`;
        activeIframe.style.cssText = "position:absolute;top:0;left:0;width:100%;height:100%;border:none;z-index:10;pointer-events:none;";
        container.appendChild(activeIframe);
    }

    document.addEventListener('mouseout', (e) => {
        clearTimeout(hoverTimeout);
        if (activeIframe && !e.target.closest('.card')) {
            activeIframe.remove();
            activeIframe = null;
        }
    });
})();