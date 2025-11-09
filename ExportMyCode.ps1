# ExportMyCode.ps1
# همه فایل‌های سورس را استخراج می‌کند و به ۵ فایل مساوی تقسیم می‌کند.
# همچنین فایل project_tree.txt شامل ساختار پروژه را می‌سازد.

param()

$rootDir     = $PSScriptRoot
$outDir      = Join-Path $rootDir "ExportedCode"
$include     = @("*.cs", "*.cshtml", "*.razor", "*.js", "*.ts", "*.css", "*.scss", "*.html", "*.json", "*.xml", "*.config")
$excludeDirs = @("bin", "obj", ".vs", ".git", "packages", "node_modules", ".vscode", "TestResults", "AppPackages","DOC")

# پاک‌سازی پوشه خروجی
if (Test-Path $outDir) { Remove-Item -Path $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir | Out-Null

# جمع‌آوری فایل‌های سورس
$files = Get-ChildItem -Path $rootDir -Include $include -Recurse -File |
         Where-Object {
             $full = $_.FullName
             ($excludeDirs | ForEach-Object { $full -notlike "*\$_\*" }) -notcontains $false
         }

Write-Host "Found $($files.Count) source files to export:`n"

$files | ForEach-Object {
    Write-Host "  - $($_.FullName.Substring($rootDir.Length).TrimStart('\'))"
}

# تقسیم فایل‌ها به ۵ بخش
$totalFiles = $files.Count
if ($totalFiles -eq 0) { Write-Host "No files found!"; exit }

$batchSize = [math]::Ceiling($totalFiles / 4)
$batch = 1
$index = 0

while ($index -lt $totalFiles) {
    $output = @()
    $subset = $files[$index..([math]::Min($index + $batchSize - 1, $totalFiles - 1))]
    foreach ($file in $subset) {
        $relPath = $file.FullName.Substring($rootDir.Length).TrimStart('\')
        $header  = "===== FILE: $relPath ====="
        $content = Get-Content $file.FullName -Raw
        $output += "$header`n$content`n"
    }
    $outFile = Join-Path $outDir ("Batch_{0:D2}.txt" -f $batch)
    $output | Out-File -FilePath $outFile -Encoding UTF8
    Write-Host "Created $outFile with $($subset.Count) files"
    $index += $batchSize
    $batch++
}

# ساخت درخت فقط برای فایل‌های استخراج‌شده
$treeLines = @()
foreach ($file in $files) {
    $rel   = $file.FullName.Substring($rootDir.Length).TrimStart('\')
    $parts = $rel.Split([IO.Path]::DirectorySeparatorChar)
    for ($i = 0; $i -lt $parts.Count; $i++) {
        $prefix = if ($i -eq 0) { "" } else { ("|   " * ($i - 1)) + "+-- " }
        $line   = $prefix + $parts[$i]
        if ($treeLines -notcontains $line) { $treeLines += $line }
    }
}
$treeFile = Join-Path $outDir "project_tree.txt"
$treeLines | Out-File -FilePath $treeFile -Encoding UTF8

Write-Host "`nAll done. Exported to: $outDir"
