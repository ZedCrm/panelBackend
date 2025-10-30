# ExportMyCode.ps1
# فقط فایل‌های سورسِ خودت را استخراج می‌کند و هر ۱۰‌تایی در یک فایل txt می‌ریزد.
# یک فایل project_tree.txt هم می‌سازد که فقط همان فایل‌های استخراج‌شده و پوشه‌هایشان را نشان می‌دهد.

param()

$rootDir     = $PSScriptRoot
$outDir      = Join-Path $rootDir "ExportedCode"
$include     = @("*.cs", "*.cshtml", "*.razor", "*.js", "*.ts", "*.css", "*.scss", "*.html", "*.json", "*.xml", "*.config")
$excludeDirs = @("bin", "obj", ".vs", ".git", "packages", "node_modules", ".vscode", "TestResults", "AppPackages")

# پاک‌سازی پوشه خروجی
if (Test-Path $outDir) { Remove-Item -Path $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir | Out-Null

# جمع‌آوری فایل‌های سورس
$files = Get-ChildItem -Path $rootDir -Include $include -Recurse -File |
         Where-Object {
             $full = $_.FullName
             ($excludeDirs | ForEach-Object { $full -notlike "*\$_\*" }) -notcontains $false
         }

Write-Host "Found $($files.Count) source files to export:"
$files | ForEach-Object {
    Write-Host "  - $($_.FullName.Substring($rootDir.Length).TrimStart('\'))"
}

# ذخیره دسته‌های ۱۰‌تایی
$counter = 0
$batch   = 1
$output  = @()

foreach ($file in $files) {
    $relPath = $file.FullName.Substring($rootDir.Length).TrimStart('\')
    $header  = "===== FILE: $relPath ====="
    $content = Get-Content $file.FullName -Raw
    $output += "$header`n$content`n"

    $counter++
    if ($counter -eq 10) {
        $outFile = Join-Path $outDir ("Batch_{0:D2}.txt" -f $batch)
        $output | Out-File -FilePath $outFile -Encoding UTF8
        $output = @(); $counter = 0; $batch++
    }
}

if ($output.Count -gt 0) {
    $outFile = Join-Path $outDir ("Batch_{0:D2}.txt" -f $batch)
    $output | Out-File -FilePath $outFile -Encoding UTF8
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