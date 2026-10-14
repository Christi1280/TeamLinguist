Mi Camino road tiles V3 - 64x64 RGBA PNG, roads have transparent pixels outside asphalt.
16 tiles use NESW 4-bit masks: N=1 E=2 S=4 W=8. A road tile joins another tile when the matching sides connect.
Import individual PNGs as Sprite Mode Single, PPU 64, Filter Point, Compression None, Mesh Full Rect.
Unity Tilemap sorting: Ground (0), Sidewalks (1), Roads (2), Car (3).
Sidewalk tiles are optional, separately transparent in road footprint. Grass ground is fully opaque.
IMPORTANT: Road tiles are 48 pixels wide within each 64px grid cell. Roads and sidewalks use same tile indices and cell positions.
The palette order 00-15 is bitmask order, NOT alphabetical.
Preview depicts connected tiles.
