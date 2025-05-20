<?php

$rootDir = __DIR__;
$outputFile = $rootDir . '/out.' . time() . '.cs';

$files = new RecursiveIteratorIterator(
    new RecursiveDirectoryIterator($rootDir, FilesystemIterator::SKIP_DOTS)
);

file_put_contents($outputFile, ''); // Clear or create the output file

foreach ($files as $file) {
    $filePath = $file->getPathname();
    $fileName = $file->getFilename();

    // Skip output snapshots like out.1234567890.cs
    if (preg_match('/^out\.\d+\.cs$/', $fileName)) {
        continue;
    }

    if (pathinfo($filePath, PATHINFO_EXTENSION) === 'cs') {
        $content = file_get_contents($filePath);
        file_put_contents($outputFile, "// File: $filePath\n$content\n\n", FILE_APPEND);
    }
}

echo "Snapshot created: $outputFile\n";