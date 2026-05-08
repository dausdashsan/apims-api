# Air Pollutant Index Management System (APIMS) API

## Overview

A .NET 8 Web API that wraps the Malaysian Department of Environment (DOE) real-time air quality monitoring endpoint. The upstream API returns readings from 70 monitoring stations across Malaysia. This wrapper provides filtered access via simple REST endpoints with interactive Swagger documentation.

**Upstream Source:**
```
https://eqms.doe.gov.my/api3/publicmapproxy/PUBLIC_DISPLAY/CAQM_MCAQM_Current_Reading/MapServer/0/query
```

---

## Project Architecture

```
APIMS_Api/
├── APIMS_Api.sln
├── APIMS_Api/
│   ├── APIMS_Api.csproj
│   ├── Program.cs                     # DI & Swagger configuration
│   ├── appsettings.json               # API config & cache settings
│   ├── Controllers/
│   │   └── StationsController.cs      # 5 REST endpoints
│   ├── Models/
│   │   ├── StationAttributes.cs       # 16 air quality fields
│   │   ├── StationFeature.cs          # wraps attributes
│   │   └── AqiApiResponse.cs          # top-level response
│   └── Services/
│       ├── IAqiService.cs
│       └── AqiService.cs              # HttpClient + IMemoryCache
```

---

## API Endpoints

| Method | Route | Purpose | Query Parameter |
|--------|-------|---------|-----------------|
| GET | `/api/stations` | Get all 70 stations | — |
| GET | `/api/stations/by-state` | Filter by state | `stateName` (e.g., `Selangor`) |
| GET | `/api/stations/by-region` | Filter by region | `regionName` (e.g., `Northern`) |
| GET | `/api/stations/by-class` | Filter by air quality class | `className` (e.g., `Good`) |
| GET | `/api/stations/by-station` | Filter by station ID | `stationId` (e.g., `CA01R`) |

### Response Format

All endpoints return an array of station features:
```json
[
  {
    "attributes": {
      "STATION_ID": "CA01R",
      "DATETIME": 1778270400000,
      "API": 37.0,
      "CLASS": "Good",
      "STATE_NAME": "Perlis",
      "REGION_NAME": "Northern",
      "STATION_LOCATION": "Kangar, PERLIS",
      "LONGITUDE": 100.210937,
      "LATITUDE": 6.429928,
      "PLACE": "Institut Latihan Perindustrian (Kangar)",
      "STATION_CATEGORY": "Sub Urban",
      ... (10 more fields)
    }
  }
]
```

---

## Station Attributes (16 Fields)

| Field | Type | Description |
|-------|------|-------------|
| `STATION_ID` | string | Unique station identifier |
| `DATETIME` | long | Unix epoch milliseconds |
| `API` | double? | Air Pollutant Index value |
| `API_PM10` | int? | PM10 API value (often null) |
| `PARAM_SELECTED` | string? | Primary pollutant (e.g., PM2.5) |
| `PARAM_SYMBOL` | string? | Pollutant indicator symbol |
| `PARAM_SYMBOL_PM10` | string? | PM10 symbol |
| `CLASS` | string? | Air quality class (Good, Moderate, Unhealthy, etc.) |
| `STATION_LOCATION` | string? | City/location name |
| `LONGITUDE` | double? | Geographic longitude |
| `LATITUDE` | double? | Geographic latitude |
| `PLACE` | string? | Facility name |
| `LOT_INFO` | string? | Lot/location details |
| `STATION_CATEGORY` | string? | Suburban/Urban classification |
| `STATE_NAME` | string? | Malaysian state |
| `REGION_NAME` | string? | Region (Northern, Central, Southern, etc.) |

---

## Swagger Documentation

Swagger UI is available at `/swagger` with:
- Full endpoint descriptions
- Real parameter examples:
  - **States**: Selangor, Kedah, Johor, WP Kuala Lumpur, Perlis, Sarawak, Sabah, etc.
  - **Regions**: Northern, Central, Southern, Eastern, Sabah, Sarawak
  - **Classes**: Good, Moderate, Unhealthy, Very Unhealthy, Hazardous
  - **Station IDs**: CA01R, CA02K, CA04K, etc.
- Request/response schemas
- Try-it-out feature

---

## Implementation Details

### Caching Strategy
- **Duration**: 5 minutes
- **Method**: IMemoryCache
- **Rationale**: Air quality readings update infrequently; caching reduces external API calls from 70+ per second to 1 per 5 minutes

### Data Flow
1. Client requests endpoint (e.g., `/api/stations/by-state?stateName=Selangor`)
2. Controller calls `AqiService.GetByStateAsync()`
3. Service checks cache for `"aqi_all"` key
   - **Cache hit**: Filter cached features in-memory
   - **Cache miss**: Fetch from upstream, deserialize, cache, then filter
4. Return filtered `List<StationFeature>` to client (200 OK) or empty list (404 Not Found)

### JSON Deserialization
- Uses `System.Text.Json` with `PropertyNameCaseInsensitive = true`
- Handles nullable fields gracefully
- No external dependencies required

### Filtering
- All filters applied in-memory after fetch
- Case-insensitive string comparisons
- Separate endpoint per filter for clean, RESTful design

---

## NuGet Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Swashbuckle.AspNetCore` | Latest | Swagger UI + OpenAPI docs |
| `Microsoft.Extensions.Caching.Memory` | Built-in | IMemoryCache implementation |

---

## Configuration (appsettings.json)

```json
{
  "AqiApi": {
    "BaseUrl": "https://eqms.doe.gov.my/api3/publicmapproxy/PUBLIC_DISPLAY/CAQM_MCAQM_Current_Reading/MapServer/0/query",
    "QueryParams": "?f=json&outFields=*&returnGeometry=false&spatialRel=esriSpatialRelIntersects&where=1%3D1",
    "CacheMinutes": 5
  }
}
```

---

## Building & Running

```powershell
# Build
dotnet build

# Run (starts on https://localhost:5001)
dotnet run

# Browse Swagger UI
Start-Process https://localhost:5001/swagger
```

---

## Testing Checklist

- [ ] `GET /api/stations` returns all 70 features
- [ ] `GET /api/stations/by-state?stateName=Selangor` returns Selangor stations only
- [ ] `GET /api/stations/by-region?regionName=Northern` returns Northern region stations
- [ ] `GET /api/stations/by-class?className=Good` returns Good class stations
- [ ] `GET /api/stations/by-station?stationId=CA01R` returns single station
- [ ] Invalid filter (e.g., `stateName=ZZZ`) returns empty array or 404
- [ ] Swagger UI loads at `/swagger` with all 5 endpoints documented
- [ ] Response includes all 16 attributes for each station
- [ ] Subsequent requests within 5 minutes use cached data
- [ ] Cache refresh after 5 minutes fetches fresh data

---

## Key Technical Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Architecture | Separate filter endpoints | Clean REST design; one concern per route |
| Filtering | In-memory after fetch | 70 records is tiny; simpler than ArcGIS WHERE encoding |
| Caching | IMemoryCache, 5-min TTL | Balances freshness vs. external API load |
| JSON parser | System.Text.Json | No extra dependencies; built-in to .NET 8 |
| Documentation | Swagger + XML comments | Rich, interactive, auto-generated docs |
| DATETIME | Unix millisecond epoch | Matches upstream API; convert to DateTimeOffset as needed |

---
