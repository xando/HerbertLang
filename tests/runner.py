import os
import argparse
import subprocess
import glob


TEST_CODE_FILE = "in.h"
TEST_STDOUT_FILE = "stdout"
TEST_STDERR_FILE = "stderr"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary", help="Interpreter Binary", required=True)
    parser.add_argument("--tests", required=True)

    args = parser.parse_args()

    for test_input_code_file in sorted(glob.glob(os.path.join(args.tests, "*", "in.h"))):

        test_name = os.path.dirname(test_input_code_file)
        if "function_arguments_1" in test_name:
            breakpoint()

        result = subprocess.run(
            [args.binary, test_input_code_file],
            capture_output=True,
            text=True
        )
        
        if result.returncode == 0:
            expected_output = open(os.path.join(test_name, TEST_STDOUT_FILE)).read()
            current_output = result.stdout.strip()
        else:
            expected_output = open(os.path.join(test_name, TEST_STDERR_FILE)).read()
            current_output = result.stderr.strip()

        print(f"Test:{test_name} ->", 'PASSED' if expected_output == current_output else 'FAILED')
    
if __name__ == "__main__":
    main()
