(function () {
    let hoverTimeout;
    let activeIframe = null;

    // 1. Listen for mouseover on all movie/series cards
    document.addEventListener('mouseover', function (e) {
        const card = e.target.closest('.card');
        if (!card || activeIframe) return;

        // Delay playback slightly to avoid accidental triggers while scrolling
        hoverTimeout = setTimeout(() => {
            const itemId = card.getAttribute('data-id');
            startTrailer(card, itemId);
        }, 800);
    });

    document.addEventListener('mouseout', function (e) {
        clearTimeout(hoverTimeout);
        const card = e.target.closest('.card');
        if (card && activeIframe) {
            stopTrailer();
        }
    });

    async function startTrailer(card, itemId) {
        // Here you would call your Jellyfin API or TMDb directly
        // For this demo, let's assume we have the TMDb ID
        const tmdbId = card.getAttribute('data-tmdb-id'); // You'll need to ensure this is in the DOM
        const youtubeKey = "6JnN1DmbqoU"; // Replace with your TMDb API fetch logic

        const container = card.querySelector('.cardImageContainer');

        activeIframe = document.createElement('iframe');
        activeIframe.src = `https://www.youtube.com/embed/${youtubeKey}?autoplay=1&mute=1&controls=0&loop=1&modestbranding=1&playlist=${youtubeKey}`;
        activeIframe.style.cssText = "position:absolute;top:0;left:0;width:100%;height:100%;border:none;pointer-events:none;z-index:10;";

        container.appendChild(activeIframe);
        container.classList.add('is-playing-trailer');
    }

    function stopTrailer() {
        if (activeIframe) {
            activeIframe.remove();
            activeIframe = null;
        }
    }
})();