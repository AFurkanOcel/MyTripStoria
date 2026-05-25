<template>
  <section class="auth-visual" aria-hidden="true">
    <div class="map-card">
      <div class="map-header">
        <span class="map-kicker">Live travel map</span>
        <strong>32 memories</strong>
      </div>

      <svg class="world-map" viewBox="0 0 920 520" role="img">
        <path class="route route-soft" d="M168 232 C285 156 410 166 516 246 S721 348 802 232" />
        <path class="route" d="M246 324 C356 236 489 222 612 292 S741 364 822 304" />

        <g class="land">
          <path d="M109 183 154 137 226 129 275 160 262 209 198 222 171 265 112 250 82 214Z" />
          <path d="M262 270 315 250 356 281 344 340 293 374 254 342Z" />
          <path d="M410 146 497 122 578 145 615 192 576 231 481 226 425 201Z" />
          <path d="M512 245 596 242 664 285 650 358 577 393 513 350 488 292Z" />
          <path d="M670 170 747 145 827 181 839 246 786 279 710 247Z" />
          <path d="M728 338 805 320 865 351 836 412 752 421 705 382Z" />
        </g>

        <g class="pin pin-completed" transform="translate(247 221)">
          <circle r="9" />
          <circle r="17" />
        </g>
        <g class="pin pin-planned" transform="translate(612 292)">
          <circle r="9" />
          <circle r="17" />
        </g>
        <g class="pin pin-completed" transform="translate(786 279)">
          <circle r="9" />
          <circle r="17" />
        </g>
        <g class="pin pin-planned" transform="translate(344 340)">
          <circle r="9" />
          <circle r="17" />
        </g>
      </svg>

      <div class="map-legend">
        <span><i class="legend-dot completed" />Completed trips</span>
        <span><i class="legend-dot planned" />Planned routes</span>
      </div>
    </div>
  </section>
</template>
